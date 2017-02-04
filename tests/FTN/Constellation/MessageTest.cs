using System;
using System.Dynamic;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace FTN.Constellation.Test
{
    [TestFixture]
    public class MessageTest
    {
        public MessageTest() 
        {

        }

        [Test]
        public void Create() {
            Message m = new Message();
            Assert.IsTrue(Guid.Empty != m.UUid);
            Assert.IsNotNull(m.Attributes);
            Assert.IsNotNull(m.Operations);
        }

        [Test]
        public void Serialize() {
            
            Message m = new Message();
            m.Type = "test-message";
            m.Attributes.Add("test0=true");
            m.Attributes.Add("test1=true");
            
            dynamic o = new ExpandoObject();
            o.obj = true;
            m.Payload = o;
            m.Operations.Add("create," + DateTime.UtcNow.ToString());
            m.LastModified = DateTime.UtcNow;
            m.Hash = "";
            
            string s = JsonConvert.SerializeObject(m);
            
            //The dynamic type is used to ensure that the
            //object contains the proper fields.
            dynamic msg = JsonConvert.DeserializeObject<dynamic>(s);
            
            Assert.IsNotNull(msg.u);
            Assert.AreEqual(1, int.Parse(msg.v.ToString()));
            Assert.AreEqual("test-message", msg.t.ToString());
            Assert.AreEqual("test0=true", msg.a[0].ToString());
            Assert.AreEqual("test1=true", msg.a[1].ToString());
            Assert.IsNotNull(msg.o);
            Assert.IsNotNull(msg.op);
            Assert.IsNotNull(msg.s);
            Assert.IsNotNull(msg.m);
            Assert.IsEmpty(msg.h);
        }

        [Test]
        public void FileDeserialize() {
            string testFile =  "tests/sample-data/valid-message.json";

            string json = File.ReadAllText(testFile);

            Message msg = JsonConvert.DeserializeObject<Message>(json);

            Assert.AreEqual("157fb2d5-db13-4e34-b777-0a3189740519",msg.UUid.ToString());
            Assert.AreEqual("test-message", msg.Type);
            Assert.AreEqual("test0=true", msg.Attributes[0]);
        }
    }
}