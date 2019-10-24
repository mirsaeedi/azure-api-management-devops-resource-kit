
using System;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class ApiTemplateResource: TemplateResource<ApiTemplateProperties>
    {
        public TemplateResource[] Resources { get; set; }
        public override string Type => ResourceType.Api;
    }

    public class ApiTemplateProperties
    {
        public string description { get; set; }
        public APITemplateAuthenticationSettings AuthenticationSettings { get; set; }
        public APiTemplateSubscriptionKeyParameterNames subscriptionKeyParameterNames { get; set; }
        public string type { get; set; }
        public string apiRevision { get; set; }
        public string apiVersion { get; set; }
        public bool? isCurrent { get; set; }
        public string apiRevisionDescription { get; set; }
        public string apiVersionDescription { get; set; }
        public string ApiVersionSetId { get; set; }
        public Nullable<bool> subscriptionRequired { get; set; }
        public string sourceApiId { get; set; }
        public string displayName { get; set; }
        public string serviceUrl { get; set; }
        public string path { get; set; }
        public string[] protocols { get; set; }
        public APITemplateAPIVersionSet apiVersionSet { get; set; }
        public string value { get; set; }
        public string format { get; set; }
        public APITemplateWSDLSelector wsdlSelector { get; set; }
        public string apiType { get; set; }
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
        public string query { get; set; }
    }

    public class APITemplateVersionSet
    {
        public string id { get; set; }
        public string description { get; set; }
        public string versioningScheme { get; set; }
        public string versionQueryName { get; set; }
        public string versionHeaderName { get; set; }
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

    public class APITemplateAPIVersionSet
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string versionQueryName { get; set; }
        public string versionHeaderName { get; set; }
        public string versioningScheme { get; set; }
    }
}