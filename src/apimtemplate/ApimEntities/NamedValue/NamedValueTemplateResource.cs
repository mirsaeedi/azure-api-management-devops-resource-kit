using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Apim.DevOps.Toolkit.ApimEntities
{
    public class NamedValueTemplateResource : TemplateResource<NamedValueProperties>
    {
        public override string Type => ResourceType.NamedValue;
    }
}
