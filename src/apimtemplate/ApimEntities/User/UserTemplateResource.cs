
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class UserTemplateResource : TemplateResource<UserProperties>
    {
        public override string Type => ResourceType.User;
    }
}
