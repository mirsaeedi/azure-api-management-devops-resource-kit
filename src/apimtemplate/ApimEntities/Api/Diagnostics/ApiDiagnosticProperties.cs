namespace Apim.DevOps.Toolkit.ApimEntities.Api.Diagnostics
{
    public class ApiDiagnosticsProperties
    {
        public string Name { get; set; }

        public string AlwaysLog { get; set; }

        public string LoggerId { get; set; }

        public string HttpCorrelationProtocol { get; set; }

        public string Verbosity { get; set; }

        public bool? LogClientIp { get; set; }

        public SamplingSettings Sampling { get; set; }

        public PipelineDiagnosticSettings Frontend { get; set; }

        public PipelineDiagnosticSettings Backend { get; set; }

        public bool? EnableHttpCorrelationHeaders { get; set; }
    }
}
