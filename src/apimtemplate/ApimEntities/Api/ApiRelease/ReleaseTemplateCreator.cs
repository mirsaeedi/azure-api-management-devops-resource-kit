using Apim.DevOps.Toolkit.ArmTemplates;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class ReleaseTemplateCreator
    {
        public ReleaseTemplateResource CreateAPIReleaseTemplateResource(ApiDeploymentDefinition api, string[] dependsOn)
        {
            string releaseName = $"release-revision-{api.ApiRevision}";
            // create release resource with properties
            ReleaseTemplateResource releaseTemplateResource = new ReleaseTemplateResource()
            {
                Name = $"[concat(parameters('ApimServiceName'), '/{api.Name}/{releaseName}')]",
                Properties = new ReleaseTemplateProperties()
                {
                    Notes = $"Release created to make revision {api.ApiRevision} current.",
                    ApiId = $"[resourceId('{ResourceType.Api}', parameters('ApimServiceName'), '{api.Name}')]"
                },
                DependsOn = dependsOn
            };
            return releaseTemplateResource;
        }
    }
}
