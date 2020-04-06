using System.Text;

namespace Apim.DevOps.Toolkit.ArmTemplates
{

    public class ApiProperties
    {
        public string Description { get; set; }
        public APITemplateAuthenticationSettings AuthenticationSettings { get; set; }
        public APiTemplateSubscriptionKeyParameterNames SubscriptionKeyParameterNames { get; set; }
        public string Type { get; set; }
        public string ApiRevision { get; set; }
        public string ApiVersion { get; set; }
        public bool? IsCurrent { get; set; }
        public string ApiRevisionDescription { get; set; }
        public string ApiVersionDescription { get; set; }
        public string ApiVersionSetId { get; set; }
        public bool? SubscriptionRequired { get; set; }
        public string SourceApiId { get; set; }
        public string DisplayName { get; set; }
        public string ServiceUrl { get; set; }
        public string Path { get; set; }
        public string[] Protocols { get; set; }
        public string Value { get; set; }
        public string Format { get; set; }
        public APITemplateWSDLSelector WsdlSelector { get; set; }
        public string ApiType { get; set; }
    }

    public class APITemplateAuthenticationSettings
    {
        public APITemplateOAuth2 OAuth2 { get; set; }
        public APITemplateOpenID openid { get; set; }
        public bool subscriptionKeyRequired { get; set; }
    }

    public class APiTemplateSubscriptionKeyParameterNames
    {
        public string header { get; set; }
        public string Query { get; set; }
    }

    public class APITemplateWSDLSelector
    {
        public string wsdlServiceName { get; set; }
        public string wsdlEndpointName { get; set; }
    }

    public class APITemplateOAuth2
    {
        public string AuthorizationServerId { get; set; }
        public string scope { get; set; }
    }

    public class APITemplateOpenID
    {
        public string openidProviderId { get; set; }
        public string[] bearerTokenSendingMethods { get; set; }
    }
}
