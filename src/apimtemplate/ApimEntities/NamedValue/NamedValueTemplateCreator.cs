using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Apim.DevOps.Toolkit.ApimEntities;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class NamedValueTemplateCreator : TemplateCreator, ITemplateCreator
    {
        public async Task<Template> Create(DeploymentDefinition creatorConfig)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();

            foreach (var backendTemplatePropeties in creatorConfig.NamedValues)
            {
                var namedValueTemplateResource = new NamedValueTemplateResource()
                {
                    Name = $"[concat(parameters('ApimServiceName'), '/{backendTemplatePropeties.Name}')]",
                    Properties = backendTemplatePropeties,
                    DependsOn = new string[] { }
                };
                resources.Add(namedValueTemplateResource);
            }

            template.Resources = resources.ToArray();
            return await Task.FromResult(template);
        }
    }
}
