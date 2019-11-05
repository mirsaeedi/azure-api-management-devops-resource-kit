using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class Template
    {
        [JsonProperty(PropertyName = "$schema")]
        public string Schema { get; set; }
        public string ContentVersion { get; set; }
        public Dictionary<string, TemplateParameterProperties> Parameters { get; set; }
        public TemplateResource[] Resources { get; set; }
    }

    public class TemplateParameterProperties {
        public string type { get; set; }
        public TemplateParameterMetadata metadata { get; set; }
        public string[] allowedValues { get; set; }
        public string defaultValue { get; set; }
        public string value { get; set; }
    }

    public class TemplateParameterMetadata {
        public string description { get; set; }
    }
}