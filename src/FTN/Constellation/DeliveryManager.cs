using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
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
        private static HttpClient hc = null;
        public static TimeSpan Timeout = new TimeSpan(0, 0, 10, 0);
        private static DeliveryManager instance = new DeliveryManager();

        public static DeliveryManager Instance
        {
            get { return instance; }
        }

        private DeliveryManager()
        {
            hc = new HttpClient();
            hc.Timeout = Timeout;
        }

        public void Run()
        {
            
        }

        public void Stop()
        {

        }

        public async static Task<bool> Deliver(Message msg, DeliveryRule dr, bool wait)
        {
            string message = JsonConvert.SerializeObject(msg);
            bool result = true;

            foreach (Uri uri in dr.TargetEndpoints)
            {
                result &= await AttemptDeliveryAsync(message, uri).ConfigureAwait(false);
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

            return await Task.WhenAll(deliveries).ConfigureAwait(false);
        }

        public static async Task<bool> AttemptDeliveryAsync(string msg, Uri uri)
        {
            bool result = false;

            try
            {
                StringContent sc = new StringContent(msg);
                sc.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                using (HttpResponseMessage hrm = await DeliveryManager.hc.PostAsync(uri, sc).ConfigureAwait(false))
                {
                    hrm.EnsureSuccessStatusCode();
                    result = true;
                    hrm.Dispose();
                }

                sc.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Unable to deliver message to {0}. Exception {1}", uri, ex));
            }

            return result;
        }
    }
}