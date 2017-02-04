using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FTN.Constellation
{
    public class DeliveryRule
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "enabled")]
        public bool Enabled { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }

        [JsonProperty(PropertyName = "targetEndpoints")]
        public List<Uri> TargetEndpoints { get; set; }

        [JsonProperty(PropertyName = "typeExpression")]
        public string TypeExpression { get; set; }
        private Regex typeRegex;

        [JsonProperty(PropertyName = "attributeExpression")]
        public string AttributeExpression { get; set; }
        private Regex attributeRegex;

        public long MatchCount { get; set; } 

        public List<string> ValidationFailures { get; set; }

        public bool IsValid
        {
            get
            {
                return Validate();
            }
        }

        public bool IsInitialized { get; set; }

        public DeliveryRule()
        {
            ValidationFailures = new List<string>();
            TargetEndpoints = new List<Uri>();
            IsInitialized = false;
        }

        public void Initialize()
        {
            if (!Validate())
                throw new Exception("The object is not valid. Errors: " + String.Join(",", this.ValidationFailures.ToArray()));

            if (!InitalizeExpressions())
                throw new Exception("The matching expression(s) are invalid. Errors: " + String.Join(",", this.ValidationFailures.ToArray()));

            MatchCount = 0;
            IsInitialized = true;
        }

        public bool InitalizeExpressions()
        {
            bool result = false;

            if (!IsValid)
                return false;

            try
            {
                if (TypeExpression != null && TypeExpression != "")
                    typeRegex = new Regex(TypeExpression, RegexOptions.Compiled, new TimeSpan(0, 0, 0, 1));

                if (AttributeExpression != null && AttributeExpression != "")
                    attributeRegex = new Regex(AttributeExpression, RegexOptions.Compiled, new TimeSpan(0, 0, 0, 1));

                result = true;
            }
            catch (Exception ex)
            {
                ValidationFailures.Add(ex.Message);
            }

            return result;
        }


        public bool Validate()
        {
            if (this.ValidationFailures == null)
                this.ValidationFailures = new List<string>();

            this.ValidationFailures.Clear();

            if (Name == null || Name == "")
                this.ValidationFailures.Add("Name is required.");

            if (TypeExpression == null || TypeExpression == "")
                this.ValidationFailures.Add("A type match is required.");

            if (TargetEndpoints == null || TargetEndpoints.Count == 0)
                this.ValidationFailures.Add("At least one destination must be specified.");

            if (this.ValidationFailures.Count > 0)
                return false;
            else
                return true;
        }

        public bool IsMatch(Message msg)
        {
            if (!IsInitialized)
                throw new Exception("A call to Initialze() is required before matching can take place.");

            if (AttributeExpression == null || AttributeExpression == "")
            {
                if (typeRegex.IsMatch(msg.Type)) {
                    MatchCount++;
                    return true;
                }
            }
            else
            {
                if(typeRegex.IsMatch(msg.Type) && attributeRegex.IsMatch(String.Join(",",msg.Attributes.ToArray()))) {
                    MatchCount++;
                    return true;
                }
            }

            return false;
        }
    }
}