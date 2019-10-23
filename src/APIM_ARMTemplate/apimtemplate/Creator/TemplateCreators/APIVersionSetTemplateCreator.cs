using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class APIVersionSetTemplateCreator : TemplateCreator,ITemplateCreator
    {
        public async Task<Template> Create(CreatorConfig creatorConfig)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();

            foreach(var apiVersionSet in creatorConfig.ApiVersionSets)
            {
                // create apiVersionSet resource with properties
                // default version set id to version set if id is not provided
                string versionSetId = (apiVersionSet != null && apiVersionSet.id != null) ? apiVersionSet.id : "versionset";
                
                var apiVersionSetTemplateResource = new APIVersionSetTemplateResource()
                {
                    Name = $"[concat(parameters('ApimServiceName'), '/{versionSetId}')]",
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

            template.resources = resources.ToArray();
            return await Task.FromResult(template);
        }
    }
}
