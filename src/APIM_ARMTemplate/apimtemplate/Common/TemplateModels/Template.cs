using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class Template
    {
        [JsonProperty(PropertyName = "$schema")]
        public string schema { get; set; }
        public string contentVersion { get; set; }
        public Dictionary<string, TemplateParameterProperties> Parameters { get; set; }
        public object variables { get; set; }
        public TemplateResource[] resources { get; set; }
        public object outputs { get; set; }
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
    
    public abstract class TemplateResource<TProperties> : TemplateResource {
        public TProperties Properties { get; set; }
    }

    public abstract class TemplateResource
    {
        public string Name { get; set; }
        public abstract string Type { get; }
        public string ApiVersion { get; set; } = GlobalConstants.ApiVersion;
        public string Scale { get; set; }
        public string[] DependsOn { get; set; }
    }
}