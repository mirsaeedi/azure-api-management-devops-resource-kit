using System.Collections.Generic;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class BackendTemplateCreator : TemplateCreator, ITemplateCreator
    {
        public Template Create(CreatorConfig creatorConfig)
        {
            // create empty template
            Template backendTemplate = CreateEmptyTemplate();

            // add parameters
            backendTemplate.parameters = new Dictionary<string, TemplateParameterProperties>
            {
                { "ApimServiceName", new TemplateParameterProperties(){ type = "string" } }
            };

            List<TemplateResource> resources = new List<TemplateResource>();
            foreach (BackendTemplateProperties backendTemplatePropeties in creatorConfig.backends)
            {
                // create backend resource with properties
                BackendTemplateResource backendTemplateResource = new BackendTemplateResource()
                {
                    name = $"[concat(parameters('ApimServiceName'), '/{backendTemplatePropeties.title}')]",
                    Type = ResourceType.Backend,
                    apiVersion = GlobalConstants.APIVersion,
                    properties = backendTemplatePropeties,
                    dependsOn = new string[] { }
                };
                resources.Add(backendTemplateResource);
            }

            backendTemplate.resources = resources.ToArray();
            return backendTemplate;
        }
    }
}
