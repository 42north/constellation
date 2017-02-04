using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FTN.Constellation
{
    public class Message
    {
        [JsonProperty("u")]
        public Guid UUid;
        [JsonPropertyAttribute(PropertyName = "v")]
        public int Version;

        [JsonPropertyAttribute(PropertyName = "p")]
        public Guid ProcessUUid;

        [JsonPropertyAttribute(PropertyName = "t")]
        public string Type;
        [JsonPropertyAttribute(PropertyName = "a")]
        public List<string> Attributes;
        [JsonConverterAttribute(typeof(ExpandoObjectConverter))]
        [JsonPropertyAttribute(PropertyName = "o")]
        public dynamic Payload;
        [JsonPropertyAttribute(PropertyName = "op")]
        public List<string> Operations;
        [JsonPropertyAttribute(PropertyName = "s")]
        public DateTime Timestamp;
        [JsonPropertyAttribute(PropertyName = "m")]
        public DateTime LastModified;
        [JsonPropertyAttribute(PropertyName = "h")]
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