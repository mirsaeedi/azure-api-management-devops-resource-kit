using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class ReleaseTemplateCreator
    {
        public ReleaseTemplateResource CreateAPIReleaseTemplateResource(ApiConfiguration api, string[] dependsOn)
        {
            string releaseName = $"release-revision-{api.apiRevision}";
            // create release resource with properties
            ReleaseTemplateResource releaseTemplateResource = new ReleaseTemplateResource()
            {
                Name = $"[concat(parameters('ApimServiceName'), '/{api.name}/{releaseName}')]",
                Properties = new ReleaseTemplateProperties()
                {
                    Notes = $"Release created to make revision {api.apiRevision} current.",
                    ApiId = $"[resourceId('{ResourceType.Api}', parameters('ApimServiceName'), '{api.name}')]"
                },
                DependsOn = dependsOn
            };
            return releaseTemplateResource;
        }
    }
}
