
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Apim.DevOps.Toolkit.ArmTemplates
{

    public class PolicyTemplateResource : TemplateResource<PolicyProperties>
    {
        private string _type;
        public PolicyTemplateResource(string resourceType)
        {
            _type = resourceType;
        }
        public override string Type => _type;
    }
}
