using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class MasterTemplateResource : TemplateResource<LinkedProperties>
    {
        public override string Type => ResourceType.Deployment;
    }
}
