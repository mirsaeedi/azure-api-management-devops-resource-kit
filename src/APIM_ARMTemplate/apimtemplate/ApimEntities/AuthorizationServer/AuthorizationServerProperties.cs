using System;
using System.Collections.Generic;
using System.Text;

namespace Apim.DevOps.Toolkit.ArmTemplates
{

    public class AuthorizationServerProperties
    {
        public string Description { get; set; }
        public string[] AuthorizationMethods { get; set; }
        public string[] ClientAuthenticationMethod { get; set; }
        public TokenBodyParameter[] TokenBodyParameters { get; set; }
        public string tokenEndpoint { get; set; }
        public bool supportState { get; set; }
        public string defaultScope { get; set; }
        public string[] bearerTokenSendingMethods { get; set; }
        public string clientSecret { get; set; }
        public string resourceOwnerUsername { get; set; }
        public string resourceOwnerPassword { get; set; }
        public string displayName { get; set; }
        public string clientRegistrationEndpoint { get; set; }
        public string authorizationEndpoint { get; set; }
        public string[] grantTypes { get; set; }
        public string ClientId { get; set; }
    }

    public class TokenBodyParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
