using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Apim.DevOps.Toolkit.ArmTemplates;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class UserTemplateCreator : TemplateCreator, ITemplateCreator
    {
        public async Task<Template> Create(DeploymentDefinition creatorConfig)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();

            foreach (var userTemplatePropeties in creatorConfig.Users)
            {
                var userTemplateResource = new UserTemplateResource()
                {
                    Name = $"[concat(parameters('ApimServiceName'), '/{userTemplatePropeties.Name}')]",
                    Properties = userTemplatePropeties,
                    DependsOn = new string[] { }
                };
                resources.Add(userTemplateResource);
            }

            template.Resources = resources.ToArray();
            return await Task.FromResult(template);
        }
	}
}
