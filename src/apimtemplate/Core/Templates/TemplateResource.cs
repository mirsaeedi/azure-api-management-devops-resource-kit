using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
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