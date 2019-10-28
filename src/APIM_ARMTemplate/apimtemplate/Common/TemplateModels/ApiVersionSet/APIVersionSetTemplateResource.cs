
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class APIVersionSetTemplateResource : TemplateResource<ApiVersionSetProperties>
    {
        public override string Type => ResourceType.ApiVersionSet;
    }
}
