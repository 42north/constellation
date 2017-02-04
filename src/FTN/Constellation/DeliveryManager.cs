using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace FTN.Constellation
{
    ///<summary>
    ///The delivery manager handles the synchronous and asynchronous delivery of messages to Delivery Rule endpoints.
    ///</summary>
    public class DeliveryManager
    {
        public static TimeSpan Timeout = new TimeSpan(0, 0, 0, 1);

        public static bool Deliver(Message msg, DeliveryRule dr)
        {
            string message = JsonConvert.SerializeObject(msg);
            bool result = false;

            foreach (Uri uri in dr.TargetEndpoints)
            {
                Task<bool> wait = AttemptDelivery(message, uri);
                wait.Wait();
                result = wait.Result;

                if (result)
                    break;
            }

            return result;
        }

        public static async Task<bool> AttemptDelivery(string msg, Uri uri)
        {
            bool result = false;
            using (HttpClient hc = new HttpClient())
            {
                hc.Timeout = DeliveryManager.Timeout;

                StringContent sc = new StringContent(msg);
                sc.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                try
                {
                    HttpResponseMessage hrm = await hc.PostAsync(uri, sc);
                    result = hrm.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    Log.Error("Unable to deliver message to " + uri);
                    Console.WriteLine(ex);
                }

                sc.Dispose();
                hc.Dispose();
            }

            return result;
        }
    }
}