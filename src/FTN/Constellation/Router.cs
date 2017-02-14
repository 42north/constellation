using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FTN.Constellation;
using Newtonsoft.Json;

namespace FTN.Constellation.Routing
{
    public class Router
    {
        private SemaphoreSlim deliverySemaphore = new SemaphoreSlim(4, 4);

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