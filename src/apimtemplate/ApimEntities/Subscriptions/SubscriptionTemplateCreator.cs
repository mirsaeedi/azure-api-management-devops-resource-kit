using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Apim.DevOps.Toolkit.ArmTemplates;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class SubscriptionTemplateCreator : TemplateCreator, ITemplateCreator
    {
        public async Task<Template> Create(DeploymentDefinition creatorConfig)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();

            foreach (var templateProperties in creatorConfig.Subscriptions)
            {
                var templateResource = new SubscriptionTemplateResource()
                {
                    Name = $"[concat(parameters('ApimServiceName'), '/{templateProperties.Name}')]",
                    Properties = templateProperties,
                    DependsOn = new string[] { }
                };
                resources.Add(templateResource);
            }

            template.Resources = resources.ToArray();
            return await Task.FromResult(template);
        }
	}
}
