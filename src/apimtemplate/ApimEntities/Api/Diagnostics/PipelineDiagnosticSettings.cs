namespace Apim.DevOps.Toolkit.ApimEntities.Api.Diagnostics
{
    public class PipelineDiagnosticSettings
    {
        public HttpMessageDiagnostic Request { get; set; }

        public HttpMessageDiagnostic Response { get; set; }
    }
}
