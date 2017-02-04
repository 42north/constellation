using System;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FTN.Constellation.Test
{
    [Route("ftn/constellation/test")]
    public class ReceiveMessageController : Controller
    {

        [RouteAttribute("receive")]
        public IActionResult Receive(Message message)
        {
            Console.WriteLine(message.UUid);

            dynamic response = new ExpandoObject();
            response.status = "ok";

            return new JsonResult(JsonConvert.SerializeObject(response));
        }
    }
}