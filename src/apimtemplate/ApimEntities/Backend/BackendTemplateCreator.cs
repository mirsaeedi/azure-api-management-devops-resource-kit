using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Apim.DevOps.Toolkit.ArmTemplates;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class BackendTemplateCreator : TemplateCreator, ITemplateCreator
    {
        public async Task<Template> Create(DeploymentDefinition creatorConfig)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();

            foreach (var backendTemplatePropeties in creatorConfig.Backends)
            {
                var backendTemplateResource = new BackendTemplateResource()
                {
                    Name = $"[concat(parameters('ApimServiceName'), '/{backendTemplatePropeties.Title}')]",
                    Properties = backendTemplatePropeties,
                    DependsOn = new string[] { }
                };
                resources.Add(backendTemplateResource);
            }

            template.Resources = resources.ToArray();
            return await Task.FromResult(template);
        }
    }
}
