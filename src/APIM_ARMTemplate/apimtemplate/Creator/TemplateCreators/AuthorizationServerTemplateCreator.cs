using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class AuthorizationServerTemplateCreator : TemplateCreator, ITemplateCreator
    {

        public async Task<Template> Create(CreatorConfig creatorConfig)
        {
            var authorizationTemplate = EmptyTemplate;
            authorizationTemplate.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();

            foreach (var authorizationServerTemplateProperties in creatorConfig.AuthorizationServers)
            {
                // create authorization server resource with properties
                var authorizationServerTemplateResource = new AuthorizationServerTemplateResource()
                {
                    Name = $"[concat(parameters('ApimServiceName'), '/{authorizationServerTemplateProperties.displayName}')]",
                    Properties = authorizationServerTemplateProperties,
                    DependsOn = new string[] { }
                };
                resources.Add(authorizationServerTemplateResource);
            }

            authorizationTemplate.resources = resources.ToArray();

            return await Task.FromResult(authorizationTemplate);
        }
    }
}
