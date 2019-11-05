using System;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class DiagnosticTemplateResource : TemplateResource<DiagnosticTemplateProperties>
    {
        public override string Type => ResourceType.ApiDiagnostic;
    }

    public class DiagnosticTemplateProperties
    {
        public string AlwaysLog { get; set; }
        public string LoggerId { get; set; }
        public DiagnosticTemplateSampling Sampling { get; set; }
        public DiagnosticTemplateFrontendBackend Frontend { get; set; }
        public DiagnosticTemplateFrontendBackend Backend { get; set; }
        public bool? EnableHttpCorrelationHeaders { get; set; }
    }

    public class DiagnosticTemplateSampling
    {
        public string SamplingType { get; set; }
        public double Percentage { get; set; }
    }

    public class DiagnosticTemplateFrontendBackend
    {
        public DiagnosticTemplateRequestResponse request { get; set; }
        public DiagnosticTemplateRequestResponse response { get; set; }
    }

    public class DiagnosticTemplateRequestResponse
    {
        public string[] headers { get; set; }
        public DiagnosticTemplateRequestResponseBody body { get; set; }
    }

    public class DiagnosticTemplateRequestResponseBody
    {
        public int Bytes { get; set; }
    }




}
