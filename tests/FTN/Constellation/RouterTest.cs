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
            ReceiveServer.Start();
            
            Message m = new Message();
            m.Type = "valid-test-1";

            Assert.IsTrue(Router.Deliver(m, true));
        }
        
    }
}