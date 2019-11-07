
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class ReleaseTemplateResource : TemplateResource<ReleaseTemplateProperties>
    {
        public override string Type => ResourceType.ApiRelease;
    }
}