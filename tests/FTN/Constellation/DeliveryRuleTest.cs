using System;
using System.Dynamic;
using NUnit.Framework;

namespace FTN.Constellation.Test
{
    [TestFixtureAttribute]
    public class DeliveryRuleTest
    {

        public static string InvalidJson = "configuration/test-configurations/invalid-rule.json";
        public static string InvalidListJson = "configuration/test-configurations/invalid-list-rule.json";
        public static string ValidListJson = "configuration/test-configurations/invalid-list-rule.json";

        public DeliveryRuleTest()
        {

        }

        [TestAttribute]
        public void TestNew()
        {
            DeliveryRule dr = new DeliveryRule();
            Assert.IsNotNull(dr.TargetEndpoints);
            Assert.IsNotNull(dr.ValidationFailures);
        }

        [TestAttribute]
        public void TestValidationSuccess()
        {
            DeliveryRule dr = new DeliveryRule();

            //the required field for the object.
            dr.Name = "my-delivery-test";
            dr.TargetEndpoints.Add(new Uri("http://test.dev.42n.co"));
            dr.TypeExpression = "/sample-delivery-rule/g";

            Assert.IsTrue(dr.IsValid);
        }

        [TestAttribute]
        public void TestInitalizationSuccess()
        {
            DeliveryRule dr = new DeliveryRule();

            //the required field for the object.
            dr.Name = "my-delivery-test";
            dr.TargetEndpoints.Add(new Uri("http://test.dev.42n.co"));
            dr.TypeExpression = "/sample-delivery-rule/g";

            dr.Initialize();
            
            Assert.IsTrue(dr.IsInitialized);
            Assert.IsTrue(dr.IsValid);
        }


        [TestAttribute]
        public void TestInitalizationFailureRequiredFields()
        {
            DeliveryRule dr = new DeliveryRule();

            Assert.Throws<Exception>(() => {
                dr.Initialize();
            });
            
            Assert.AreEqual(3, dr.ValidationFailures.Count);
        }

        [TestAttribute]
        public void TestInitalizationFailureRulesExpressionError()
        {
            DeliveryRule dr = new DeliveryRule();

            //the required field for the object.
            dr.Name = "my-name";
            dr.TargetEndpoints.Add(new Uri("http://test.dev.42n.co"));
            dr.TypeExpression = @"\b[a\t]\w+\\\";

            Assert.Throws<Exception>(() => {
                dr.Initialize();
            });
            
            Assert.AreEqual(1, dr.ValidationFailures.Count);
        }

        [TestAttribute]
        public void TestMessageMatchSuccess()
        {
            DeliveryRule dr = new DeliveryRule();

            Message m = new Message();
            m.Type = "my-name-2";
            m.Payload = new ExpandoObject();

            //the required field for the object.
            dr.Name = "my-rule";
            dr.TargetEndpoints.Add(new Uri("http://test.dev.42n.co"));
            dr.TypeExpression = @"^my-name-2";
            dr.Initialize();
            
            bool result = dr.IsMatch(m);

            Assert.IsTrue(result);
        }
    }
}