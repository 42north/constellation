using System;
using System.IO;
using System.Threading.Tasks;
using FTN.Constellation.Routing;
using NUnit.Framework;

namespace FTN.Constellation.Test
{
    [TestFixture]
    public class RouterTest
    {
        public RouterTest()
        {

        }

        [Test]
        public void Create()
        {
            Router r = Router.Instance;
            Assert.IsNotNull(r);
            Assert.IsNotNull(r.DeliveryRules);
        }

        [Test]
        public void LoadValidRules()
        {
            string rules = File.ReadAllText("tests/sample-data/valid-delivery-rules.json");
            int count = Router.LoadRules(rules);

            Assert.AreEqual(2, count);
        }

        [Test]
        public void LoadInvalidRules()
        {
            Router r = Router.Instance;

            string rules = File.ReadAllText("tests/sample-data/invalid-delivery-rules.json");

            Assert.Catch(() =>
            {
                int count = Router.LoadRules(rules);
            });

            Assert.AreEqual(0, r.DeliveryRules.Count);
        }

        [Test]
        public void TestRuleMatch()
        {
            string rules = File.ReadAllText("tests/sample-data/valid-delivery-rules.json");
            Router.LoadRules(rules);

            Message m = new Message();
            m.Type = "valid-test-1";

            DeliveryRule dr = Router.IsMatch(m);

            Assert.IsNotNull(dr);
            Assert.AreEqual("valid-test-delivery-1", dr.Name);
        }


        [Test]
        public void TestRouter()
        {
            string rules = File.ReadAllText("tests/sample-data/valid-delivery-rules.json");
            Router.LoadRules(rules);

            dynamic server = ReceiveServer.Start();

            Message m = new Message();
            m.Type = "valid-test-1";

            bool deliveryValue = Router.Deliver(m, true);

            server.Dispose();

            Assert.IsTrue(deliveryValue);
        }

        [Test]
        public async Task TestRouterLarge()
        {
            string rules = File.ReadAllText("tests/sample-data/valid-delivery-rules.json");
            Router.LoadRules(rules);

            dynamic server = ReceiveServer.Start();

            Message m = new Message();
            m.Type = "valid-test-1";

            for (int i = 0; i < 10000; i++)
            {
                await Router.DeliverAsync(m);
            }

            server.Dispose();

            return;
        }

        [Test]
        public async Task TestRouterQueueLarge()
        {
            string rules = File.ReadAllText("tests/sample-data/valid-delivery-rules.json");
            Router.LoadRules(rules);

            dynamic server = ReceiveServer.Start();

            Message m = new Message();
            m.Type = "valid-test-1";

            for (int i = 0; i < 10000; i++)
            {
                Router.QueueForDelivery(m);
            }

            while (Router.Instance.MessageQueue.Count > 0)
                await Task.Delay(1000);

            server.Dispose();

            return;
        }

        [Test]
        public void TestRouterAsync()
        {
            string rules = File.ReadAllText("tests/sample-data/valid-delivery-rules.json");
            Router.LoadRules(rules);

            dynamic server = ReceiveServer.Start();

            Message m = new Message();
            m.Type = "valid-test-1";

            try
            {
                Router.DeliverAsync(m).Wait();
            }
            finally
            {
                server.Dispose();
            }
        }

        [Test]
        public void TestRouterNoRule()
        {
            string rules = File.ReadAllText("tests/sample-data/valid-delivery-rules.json");
            Router.LoadRules(rules);

            dynamic server = ReceiveServer.Start();

            Message m = new Message();
            m.Type = "unknown-rule";

            bool deliveryValue = Router.Deliver(m, true);

            server.Dispose();

            Assert.IsFalse(deliveryValue);
        }
    }
}