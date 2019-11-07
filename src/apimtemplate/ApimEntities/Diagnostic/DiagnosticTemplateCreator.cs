using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class DiagnosticTemplateCreator
    {
        public DiagnosticTemplateResource CreateAPIDiagnosticTemplateResource(ApiDeploymentDefinition api, string[] dependsOn)
        {
            // create diagnostic resource with properties
            DiagnosticTemplateResource diagnosticTemplateResource = new DiagnosticTemplateResource()
            {
                Name = $"[concat(parameters('ApimServiceName'), '/{api.Name}/{api.Diagnostic.Name}')]",
                Properties = new DiagnosticTemplateProperties()
                {
                    AlwaysLog = api.Diagnostic.AlwaysLog,
                    Sampling = api.Diagnostic.Sampling,
                    Frontend = api.Diagnostic.Frontend,
                    Backend = api.Diagnostic.Backend,
                    EnableHttpCorrelationHeaders = api.Diagnostic.EnableHttpCorrelationHeaders
                },
                DependsOn = dependsOn
            };
            // reference the provided logger if loggerId is provided
            if (api.Diagnostic.LoggerId != null)
            {
                diagnosticTemplateResource.Properties.LoggerId = $"[resourceId('{ResourceType.Logger}', parameters('ApimServiceName'), '{api.Diagnostic.LoggerId}')]";
            }
            return diagnosticTemplateResource;
        }
    }
}
