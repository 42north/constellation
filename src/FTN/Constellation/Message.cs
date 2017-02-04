using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FTN.Constellation
{
    public class Message
    {
        [JsonProperty("u")]
        public Guid UUid;
        [JsonPropertyAttribute(PropertyName = "v")]
        public int Version;
        [JsonPropertyAttribute(PropertyName = "t")]
        public string Type;
        [JsonPropertyAttribute(PropertyName = "a")]
        public List<string> Attributes;
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
        }
    }
}