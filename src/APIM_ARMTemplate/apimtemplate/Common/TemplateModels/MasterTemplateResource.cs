using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class MasterTemplateResource : TemplateResource<MasterTemplateProperties>
    {
        public override string Type => ResourceType.Deployment;
    }

    public class MasterTemplateProperties
    {
        public string Mode { get; set; }
        public MasterTemplateLink TemplateLink { get; set; }
        public Dictionary<string, TemplateParameterProperties> Parameters { get; set; }
    }

    public class MasterTemplateLink
    {
        public string Uri { get; set; }
        public string ContentVersion { get; set; }
    }
}
