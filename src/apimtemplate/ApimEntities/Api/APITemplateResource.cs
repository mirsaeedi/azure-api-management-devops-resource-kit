
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class ApiTemplateResource: TemplateResource<ApiProperties>
    {
        public TemplateResource[] Resources { get; set; }
        public override string Type => ResourceType.Api;
    }
}