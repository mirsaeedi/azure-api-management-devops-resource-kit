
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class AuthorizationServerTemplateResource : TemplateResource<AuthorizationServerProperties>
    {
        public override string Type => ResourceType.AuthorizationServer;
    }


}
