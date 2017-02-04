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
        public static TimeSpan Timeout = new TimeSpan(0, 0, 10, 0);

        public static bool Deliver(Message msg, DeliveryRule dr, bool wait)
        {
            string message = JsonConvert.SerializeObject(msg);
            bool result = false;

            foreach (Uri uri in dr.TargetEndpoints)
            {
                Task<bool> waitTask = AttemptDelivery(message, uri, wait);

                if (wait)
                {
                    waitTask.Wait();
                    result = waitTask.Result;
                }
            }

            return result;
        }

        public static async Task<bool> AttemptDelivery(string msg, Uri uri, bool wait)
        {
            bool result = false;
            using (HttpClient hc = new HttpClient())
            {
                hc.Timeout = DeliveryManager.Timeout;

                StringContent sc = new StringContent(msg);
                sc.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                try
                {
                    if (wait)
                    {
                        HttpResponseMessage hrm = await hc.PostAsync(uri, sc);
                        hrm.EnsureSuccessStatusCode();
                    }
                    else
                    {
                        HttpResponseMessage hrm = await hc.PostAsync(uri, sc);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(string.Format("Unable to deliver message to {0}. Exception {1}", uri, ex));
                }

                sc.Dispose();
                hc.Dispose();
            }

            return result;
        }
    }
}