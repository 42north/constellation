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
        private Task queueProcessor = null;
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

            queueProcessor = Task.Factory.StartNew(() => { 
                ProcessQueue();
            });
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
                Router.Instance.deliverySemaphore.Wait();

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

            //try
            //{
                if (Router.Instance.processQueueSemaphore.CurrentCount < 2)
                    Router.Instance.processQueueSemaphore.Release();
            //}
            //catch (Exception ex)
            //{
            //    Log.Error(string.Format("Release error {0}", ex));
            //}
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
                            await DeliverAsync(message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("Queue process error {0}", ex));
                }

                await processQueueSemaphore.WaitAsync(10000);
                Log.Verbose("Checking queue status");
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