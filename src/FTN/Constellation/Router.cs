using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FTN.Constellation;
using Newtonsoft.Json;
using Serilog;

namespace FTN.Constellation.Routing
{
    public class Router
    {
        private Thread queueProcessor = null;
        private SemaphoreSlim processQueueSemaphore = new SemaphoreSlim(1);
        private SemaphoreSlim deliverySemaphore = new SemaphoreSlim(8, 8);
        public ConcurrentQueue<Message> MessageQueue = new ConcurrentQueue<Message>();

        public RouterStatistics Statistics { get; set; }

        List<DeliveryRule> rules;
        public List<DeliveryRule> DeliveryRules
        {
            get
            {
                return rules;
            }
        }

        private static Router instance;
        public static Router Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Router();
                }

                return instance;
            }
        }

        private Router()
        {
            rules = new List<DeliveryRule>();

            ThreadStart ts = new ThreadStart(ProcessQueue);
            queueProcessor = new Thread(ts);
            queueProcessor.Start();
        }

        public static int LoadRules(string ruleJSON)
        {
            List<DeliveryRule> localRules = JsonConvert.DeserializeObject<List<DeliveryRule>>(ruleJSON);

            localRules.ForEach((rule) =>
            {
                rule.Initialize();
            });

            Router.Instance.rules = localRules;

            return Router.Instance.rules.Count;
        }

        public static bool Deliver(Message msg, bool wait)
        {
            DeliveryRule dr = Router.IsMatch(msg);

            if (dr == null)
                return false;

            return DeliveryManager.Deliver(msg, dr, wait).Result;
        }

        public static async Task<dynamic> DeliverAsync(Message msg)
        {
            DeliveryRule dr = Router.IsMatch(msg);

            try
            {
                await Router.Instance.deliverySemaphore.WaitAsync().ConfigureAwait(false);
                return DeliveryManager.DeliverAsync(msg, dr).ContinueWith((result) =>
                                {
                                    Router.Instance.deliverySemaphore.Release();
                                });
            }
            catch (Exception ex)
            {
                Router.Instance.deliverySemaphore.Release();
                throw ex;
            }
        }

        public static void QueueForDelivery(Message message)
        {
            Router.Instance.MessageQueue.Enqueue(message);

            if (Router.Instance.processQueueSemaphore.CurrentCount < 1)
                Router.Instance.processQueueSemaphore.Release();
        }

        public static void QueueForDelivery(Message[] message)
        {
            for (int i = 0; i < message.Length; i++)
                Router.Instance.MessageQueue.Enqueue(message[i]);

            Router.Instance.processQueueSemaphore.Release();
        }

        public static void QueueForDelivery(List<Message> message)
        {
            for (int i = 0; i < message.Count; i++)
                Router.Instance.MessageQueue.Enqueue(message[i]);

            Router.Instance.processQueueSemaphore.Release();
        }

        public async void ProcessQueue()
        {
            while (true)
            {
                try
                {
                    while (MessageQueue.Count > 0)
                    {
                        Message message = null;

                        MessageQueue.TryDequeue(out message);

                        if (message != null)
                        {
                            await DeliverAsync(message).ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("Queue process error {0}", ex));
                }

                bool result = await processQueueSemaphore.WaitAsync(60000);

                if (!result)
                {
                    Log.Verbose("Async wait timeout - checking for messages.");
                    Log.Verbose(string.Format("Queue contains {0} messages. Queue should be empty.", MessageQueue.Count));
                }
                else
                {
                    //Log.Verbose("Async release - checking for messages.");
                }
            }
        }

        public void MessageDeliveryFailure()
        {

        }

        public void MessageDeliverySuccess()
        {

        }

        //Handles inspection of the Rules 
        public static DeliveryRule IsMatch(Message msg)
        {
            DeliveryRule rule = Router.Instance.rules.Find((r) =>
            {
                return r.IsMatch(msg);
            });

            if (rule != null && !rule.Enabled)
                rule = null;

            return rule;
        }
    }
}