using FTN.Constellation.Routing;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FTN.Constellation.REST
{
    [Route("0/ftn/constellation")]
    public class RouterController
    {
        [RouteAttribute("addRule")]
        public IActionResult AddRule(DeliveryRule dr)
        {
            dr.Initialize();

            Router.Instance.DeliveryRules.Add(dr);

            return new JsonResult("");
        }

        [RouteAttribute("removeRule")]
        public IActionResult removeRule(DeliveryRule dr)
        {
            Router.Instance.DeliveryRules.Remove(Router.Instance.DeliveryRules.Find((rule) =>
            {
                return (dr.Name == rule.Name);
            }));

            return new JsonResult("");
        }

        [RouteAttribute("listRules")]
        public IActionResult listRules()
        {
            return new JsonResult(JsonConvert.SerializeObject(Router.Instance.DeliveryRules));
        }
    }
}
//load rules
//add rules
//remove rules
//retrive statistics
//clear stats
//receive message
