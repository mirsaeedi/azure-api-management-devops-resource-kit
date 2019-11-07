using Xunit;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Test
{
    public class BackendTemplateCreatorTests
    {
        [Fact]
        public void ShouldCreateBackendTemplateFromCreatorConfig()
        {
            // arrange
            BackendTemplateCreator backendTemplateCreator = new BackendTemplateCreator();
            DeploymentDefinition creatorConfig = new DeploymentDefinition() { Backends = new List<BackendTemplateProperties>() };
            BackendTemplateProperties backend = new BackendTemplateProperties()
            {
                Title = "title",
                Description = "description",
                ResourceId = "resourceId",
                Url = "url",
                Protocol = "protocol",
                Proxy = new Proxy()
                {
                    url = "url",
                    username = "user",
                    password = "pass"
                },
                Tls = new Tls()
                {
                    validateCertificateChain = true,
                    validateCertificateName = true
                },
                Credentials = new Credentials()
                {
                    Certificate = new string[] { "cert1" },
                    Query = new object(),
                    Header = new object(),
                    Authorization = new CredentialsAuthorization()
                    {
                        scheme = "scheme",
                        parameter = "parameter"
                    }
                },
                Properties = new Properties()
                {
                    ServiceFabricCluster = new ServiceFabricCluster()
                    {
                        ClientCertificatethumbprint = "",
                        ManagementEndpoints = new string[] { "endpoint" },
                        MaxPartitionResolutionRetries = 1,
                        ServerCertificateThumbprints = new string[] { "thumbprint" },
                        ServerX509Names = new ServerX509Names[]{
                        new ServerX509Names(){
                            Name = "name",
                            issuerCertificateThumbprint = "thumbprint"
                        } }
                    }
                }

            };
            creatorConfig.Backends.Add(backend);

            // act
            Template backendTemplate = backendTemplateCreator.CreateBackendTemplate(creatorConfig);
            BackendTemplateResource backendTemplateResource = (BackendTemplateResource)backendTemplate.Resources[0];

            // assert
            Assert.Equal($"[concat(parameters('ApimServiceName'), '/{backend.Title}')]", backendTemplateResource.Name);
            Assert.Equal(backend.Title, backendTemplateResource.properties.title);
            Assert.Equal(backend.Description, backendTemplateResource.properties.description);
            Assert.Equal(backend.ResourceId, backendTemplateResource.properties.resourceId);
            Assert.Equal(backend.Url, backendTemplateResource.properties.url);
            Assert.Equal(backend.Protocol, backendTemplateResource.properties.protocol);
            Assert.Equal(backend.Proxy.url, backendTemplateResource.properties.proxy.url);
            Assert.Equal(backend.Proxy.username, backendTemplateResource.properties.proxy.username);
            Assert.Equal(backend.Proxy.password, backendTemplateResource.properties.proxy.password);
            Assert.Equal(backend.Tls.validateCertificateChain, backendTemplateResource.properties.tls.validateCertificateChain);
            Assert.Equal(backend.Tls.validateCertificateName, backendTemplateResource.properties.tls.validateCertificateName);
            Assert.Equal(backend.Credentials.Certificate, backendTemplateResource.properties.credentials.certificate);
            Assert.Equal(backend.Credentials.Query, backendTemplateResource.properties.credentials.query);
            Assert.Equal(backend.Credentials.Header, backendTemplateResource.properties.credentials.header);
            Assert.Equal(backend.Credentials.Authorization.scheme, backendTemplateResource.properties.credentials.authorization.scheme);
            Assert.Equal(backend.Credentials.Authorization.parameter, backendTemplateResource.properties.credentials.authorization.parameter);
            Assert.Equal(backend.Properties.ServiceFabricCluster.ClientCertificatethumbprint, backendTemplateResource.properties.properties.serviceFabricCluster.clientCertificatethumbprint);
            Assert.Equal(backend.Properties.ServiceFabricCluster.ManagementEndpoints, backendTemplateResource.properties.properties.serviceFabricCluster.managementEndpoints);
            Assert.Equal(backend.Properties.ServiceFabricCluster.MaxPartitionResolutionRetries, backendTemplateResource.properties.properties.serviceFabricCluster.maxPartitionResolutionRetries);
            Assert.Equal(backend.Properties.ServiceFabricCluster.ServerCertificateThumbprints, backendTemplateResource.properties.properties.serviceFabricCluster.serverCertificateThumbprints);
            Assert.Equal(backend.Properties.ServiceFabricCluster.ServerX509Names[0].issuerCertificateThumbprint, backendTemplateResource.properties.properties.serviceFabricCluster.serverX509Names[0].issuerCertificateThumbprint);
            Assert.Equal(backend.Properties.ServiceFabricCluster.ServerX509Names[0].Name, backendTemplateResource.properties.properties.serviceFabricCluster.serverX509Names[0].name);
        }
    }
}
