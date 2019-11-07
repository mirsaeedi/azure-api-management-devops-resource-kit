using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Apim.DevOps.Toolkit.ArmTemplates;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class APIVersionSetTemplateCreator : TemplateCreator,ITemplateCreator
    {
        public async Task<Template> Create(DeploymentDefinition creatorConfig)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();

            foreach(var apiVersionSet in creatorConfig.ApiVersionSets)
            {   
                var apiVersionSetTemplateResource = new APIVersionSetTemplateResource()
                {
                    Name = $"[concat(parameters('ApimServiceName'), '/{apiVersionSet.Name}')]",
                    Properties = new ApiVersionSetProperties()
                    {
                        DisplayName = apiVersionSet.DisplayName,
                        Description = apiVersionSet.Description,
                        VersionHeaderName = apiVersionSet.VersionHeaderName,
                        VersionQueryName = apiVersionSet.VersionQueryName,
                        VersioningScheme = apiVersionSet.VersioningScheme,
                    },
                    DependsOn = new string[] { }
                };

                resources.Add(apiVersionSetTemplateResource);
            }

            template.Resources = resources.ToArray();
            return await Task.FromResult(template);
        }
    }
}
