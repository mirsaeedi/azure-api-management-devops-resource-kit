namespace Apim.DevOps.Toolkit.ApimEntities.Api.Diagnostics
{
    public class HttpMessageDiagnostic
    {
        public string[] Headers { get; set; }

        public BodyDiagnosticSettings Body { get; set; }
    }
}
