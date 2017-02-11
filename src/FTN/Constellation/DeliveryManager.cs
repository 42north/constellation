using System;
using System.Collections.Generic;
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

        public async static Task<bool> Deliver(Message msg, DeliveryRule dr, bool wait)
        {
            string message = JsonConvert.SerializeObject(msg);
            bool result = false;

            foreach (Uri uri in dr.TargetEndpoints)
            {
                Task<bool> waitTask = AttemptDeliveryAsync(message, uri);

                if (wait)
                {
                    result = await waitTask;
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }

        public static async Task<bool[]> DeliverAsync(Message msg, DeliveryRule dr)
        {
            string message = JsonConvert.SerializeObject(msg);

            List<Task<bool>> deliveries = new List<Task<bool>>();

            foreach (Uri uri in dr.TargetEndpoints)
            {
                deliveries.Add(AttemptDeliveryAsync(message, uri));
            }

            return await Task.WhenAll(deliveries);
        }

        public static async Task<bool> AttemptDeliveryAsync(string msg, Uri uri)
        {
            bool result = false;

            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    hc.Timeout = DeliveryManager.Timeout;

                    StringContent sc = new StringContent(msg);
                    sc.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                    HttpResponseMessage hrm = await hc.PostAsync(uri, sc);
                    hrm.EnsureSuccessStatusCode();
                    result = true;
                    hrm.Dispose();

                    sc.Dispose();
                    hc.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Unable to deliver message to {0}. Exception {1}", uri, ex));
            }

            return result;
        }
    }
}