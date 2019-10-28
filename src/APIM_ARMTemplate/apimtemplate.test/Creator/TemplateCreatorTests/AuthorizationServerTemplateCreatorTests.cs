using Xunit;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Test
{
    public class AuthorizationServerTemplateCreatorTests
    {
        [Fact]
        public void ShouldCreateAuthorizationServerTemplateFromCreatorConfig()
        {
            // arrange
            AuthorizationServerTemplateCreator authorizationServerTemplateCreator = new AuthorizationServerTemplateCreator();
            CreatorConfig creatorConfig = new CreatorConfig() { AuthorizationServers = new List<AuthorizationServerTemplateProperties>() };
            AuthorizationServerTemplateProperties authorizationServer = new AuthorizationServerTemplateProperties()
            {
                Description = "description",
                displayName = "displayName",
                authorizationEndpoint = "endpoint.com",
                AuthorizationMethods = new string[] { "GET" },
                TokenBodyParameters = new AuthorizationServerTokenBodyParameter[] { new AuthorizationServerTokenBodyParameter() {
                    name = "name",
                    value = "value"
                } },
                ClientAuthenticationMethod = new string[] { "GET" },
                tokenEndpoint = "endpoint.com",
                supportState = true,
                defaultScope = "defaultScope",
                bearerTokenSendingMethods = new string[] { "GET" },
                ClientId = "id",
                clientSecret = "secret",
                clientRegistrationEndpoint = "endpoint.com",
                resourceOwnerPassword = "pass",
                resourceOwnerUsername = "user",
                grantTypes = new string[] { }
            };
            creatorConfig.AuthorizationServers.Add(authorizationServer);

            // act
            Template authorizationServerTemplate = authorizationServerTemplateCreator.CreateAuthorizationServerTemplate(creatorConfig);
            AuthorizationServerTemplateResource authorizationServerTemplateResource = (AuthorizationServerTemplateResource)authorizationServerTemplate.Resources[0];

            // assert
            Assert.Equal($"[concat(parameters('ApimServiceName'), '/{authorizationServer.displayName}')]", authorizationServerTemplateResource.Name);
            Assert.Equal(authorizationServer.Description, authorizationServerTemplateResource.properties.description);
            Assert.Equal(authorizationServer.displayName, authorizationServerTemplateResource.properties.displayName);
            Assert.Equal(authorizationServer.authorizationEndpoint, authorizationServerTemplateResource.properties.authorizationEndpoint);
            Assert.Equal(authorizationServer.AuthorizationMethods, authorizationServerTemplateResource.properties.authorizationMethods);
            Assert.Equal(authorizationServer.ClientAuthenticationMethod, authorizationServerTemplateResource.properties.clientAuthenticationMethod);
            Assert.Equal(authorizationServer.ClientId, authorizationServerTemplateResource.properties.clientId);
            Assert.Equal(authorizationServer.clientRegistrationEndpoint, authorizationServerTemplateResource.properties.clientRegistrationEndpoint);
            Assert.Equal(authorizationServer.clientSecret, authorizationServerTemplateResource.properties.clientSecret);
            Assert.Equal(authorizationServer.bearerTokenSendingMethods, authorizationServerTemplateResource.properties.bearerTokenSendingMethods);
            Assert.Equal(authorizationServer.grantTypes, authorizationServerTemplateResource.properties.grantTypes);
            Assert.Equal(authorizationServer.resourceOwnerPassword, authorizationServerTemplateResource.properties.resourceOwnerPassword);
            Assert.Equal(authorizationServer.resourceOwnerUsername, authorizationServerTemplateResource.properties.resourceOwnerUsername);
            Assert.Equal(authorizationServer.defaultScope, authorizationServerTemplateResource.properties.defaultScope);
            Assert.Equal(authorizationServer.supportState, authorizationServerTemplateResource.properties.supportState);
            Assert.Equal(authorizationServer.TokenBodyParameters[0].name, authorizationServerTemplateResource.properties.tokenBodyParameters[0].name);
            Assert.Equal(authorizationServer.TokenBodyParameters[0].value, authorizationServerTemplateResource.properties.tokenBodyParameters[0].value);
        }
    }
}
