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
                if (rules == null)
                {
                    rules = new List<DeliveryRule>();
                }

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

        }

        public int LoadRules(string ruleJSON)
        {
            List<DeliveryRule> localRules = JsonConvert.DeserializeObject<List<DeliveryRule>>(ruleJSON);

            localRules.ForEach((rule) =>
            {
                rule.Initialize();
            });

            rules = localRules;

            return rules.Count;
        }

        public bool Deliver(Message msg)
        {
            bool result = false; 

            DeliveryRule dr = IsMatch(msg);
                        
            if (dr == null)
                return false;
            
            result = DeliveryManager.Deliver(msg, dr);

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
        public DeliveryRule IsMatch(Message msg)
        {
            DeliveryRule rule = rules.Find((r) => 
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