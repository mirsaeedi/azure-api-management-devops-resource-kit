using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class PropertyTemplateResource : TemplateResource<PropertyResourceProperties>
    {
        public override string Type => ResourceType.Property;
    }

    public class PropertyResourceProperties
    {
        public IList<string> Tags { get; set; }
        public bool Secret { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }
    }
}
