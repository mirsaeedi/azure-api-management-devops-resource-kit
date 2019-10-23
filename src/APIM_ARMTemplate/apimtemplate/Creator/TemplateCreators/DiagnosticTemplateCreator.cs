using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class DiagnosticTemplateCreator
    {
        public DiagnosticTemplateResource CreateAPIDiagnosticTemplateResource(ApiConfiguration api, string[] dependsOn)
        {
            // create diagnostic resource with properties
            DiagnosticTemplateResource diagnosticTemplateResource = new DiagnosticTemplateResource()
            {
                Name = $"[concat(parameters('ApimServiceName'), '/{api.name}/{api.diagnostic.Name}')]",
                Properties = new DiagnosticTemplateProperties()
                {
                    AlwaysLog = api.diagnostic.AlwaysLog,
                    Sampling = api.diagnostic.Sampling,
                    Frontend = api.diagnostic.Frontend,
                    Backend = api.diagnostic.Backend,
                    EnableHttpCorrelationHeaders = api.diagnostic.EnableHttpCorrelationHeaders
                },
                DependsOn = dependsOn
            };
            // reference the provided logger if loggerId is provided
            if (api.diagnostic.LoggerId != null)
            {
                diagnosticTemplateResource.Properties.LoggerId = $"[resourceId('{ResourceType.Logger}', parameters('ApimServiceName'), '{api.diagnostic.LoggerId}')]";
            }
            return diagnosticTemplateResource;
        }
    }
}
