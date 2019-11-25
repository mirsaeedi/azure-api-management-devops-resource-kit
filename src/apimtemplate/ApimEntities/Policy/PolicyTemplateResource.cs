
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Apim.DevOps.Toolkit.ArmTemplates
{

    public class PolicyTemplateResource : TemplateResource<PolicyProperties>
    {
        public override string Type => ResourceType.GlobalServicePolicy;
    }
}
