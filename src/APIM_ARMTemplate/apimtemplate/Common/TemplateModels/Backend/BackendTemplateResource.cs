
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class BackendTemplateResource : TemplateResource<BackendProperties>
    {
        public override string Type => ResourceType.Backend;
    }
}
