using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FTN.Constellation;
using Newtonsoft.Json;

namespace FTN.Constellation.Routing
{
    public class Router
    {
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
            bool result = false; 

            DeliveryRule dr = Router.IsMatch(msg);
                        
            if (dr == null)
                return false;
            
            result = DeliveryManager.Deliver(msg, dr, wait);

            if (result)
            {
                //increase count success count
            } else {
                //decrease success count
            }

            return result;
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

            if (rule != null)
            {
                //matched incremement
            } else {
                //unmatched incremement
            }

            return rule;
        }
    }
}