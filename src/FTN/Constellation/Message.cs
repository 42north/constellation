using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace FTN.Constellation
{
    public class Message
    {
        [JsonPropertyName("u")]
        public Guid UUid;
        
        [JsonPropertyName("v")]
        public int Version;

        [JsonPropertyName("p")]
        public Guid ProcessUUid;

        [JsonPropertyName("t")]
        public string Type;

        [JsonPropertyName("a")]
        public List<string> Attributes;

        [JsonPropertyName("o")]
        public dynamic Payload;

        [JsonPropertyName("op")]
        public List<string> Operations;

        [JsonPropertyName("s")]

        public DateTime Timestamp;

        [JsonPropertyName("m")]
        public DateTime LastModified;

        [JsonPropertyName("h")]
        public string Hash;

        public Message()
        {
            UUid = Guid.NewGuid();
            Version = 1;
            Timestamp = DateTime.UtcNow;
            Attributes = new List<string>();
            Operations = new List<string>();
            LastModified = DateTime.UtcNow;
            //register the message with the constellation libarary.
        }

        public Message(object payload) : this()
        {
            Type = payload.GetType().FullName;
            Payload = payload;
        }

        public Message(string type, object payload) : this()
        {
            Type = type;
            Payload = payload;
        }

        public Message(Guid processGuid, string type, object payload) : this()
        {
            Type = type;
            Payload = payload;
        }

        public string FindAttribute(string attribute)
        {
            return this.Attributes.Find(x => x.Contains(attribute));
        }


        public string GetAttributeValue(string key)
        {
            string[] s = FindAttribute(key).Split('=');
            
            string v = null;

            if (s.Length == 2)
                v = s[1];

            return v;
        }
    }
}