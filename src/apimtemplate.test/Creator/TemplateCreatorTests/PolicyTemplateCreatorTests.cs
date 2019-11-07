using System.Collections.Generic;
using Xunit;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Test
{
    public class PolicyTemplateCreatorTests
    {
        [Fact]
        public void ShouldCreateGlobalServicePolicyTemplateResourceFromCreatorConfigWithCorrectContent()
        {
            // arrange
            PolicyTemplateCreator policyTemplateCreator = PolicyTemplateCreatorFactory.GeneratePolicyTemplateCreator();
            DeploymentDefinition creatorConfig = new DeploymentDefinition() { Policy = "http://someurl.com" };

            // act
            Template policyTemplate = policyTemplateCreator.CreateGlobalServicePolicyTemplate(creatorConfig);
            PolicyTemplateResource policyTemplateResource = policyTemplate.Resources[0] as PolicyTemplateResource;

            // assert
            Assert.Equal($"[concat(parameters('ApimServiceName'), '/policy')]", policyTemplateResource.Name);
            Assert.Equal("rawxml-link", policyTemplateResource.properties.format);
            Assert.Equal(creatorConfig.Policy, policyTemplateResource.properties.value);
        }

        [Fact]
        public void ShouldCreateAPIPolicyTemplateResourceFromCreatorConfigWithCorrectContent()
        {
            // arrange
            PolicyTemplateCreator policyTemplateCreator = PolicyTemplateCreatorFactory.GeneratePolicyTemplateCreator();
            DeploymentDefinition creatorConfig = new DeploymentDefinition() { Apis = new List<APIConfig>() };
            APIConfig api = new APIConfig()
            {
                name = "name",
                policy = "http://someurl.com"
            };
            creatorConfig.Apis.Add(api);
            string[] dependsOn = new string[] { "dependsOn" };

            // act
            PolicyTemplateResource policyTemplateResource = policyTemplateCreator.CreateAPIPolicyTemplateResource(api, dependsOn);

            // assert
            Assert.Equal($"[concat(parameters('ApimServiceName'), '/{api.name}/policy')]", policyTemplateResource.Name);
            Assert.Equal("rawxml-link", policyTemplateResource.properties.format);
            Assert.Equal(api.policy, policyTemplateResource.properties.value);
            Assert.Equal(dependsOn, policyTemplateResource.DependsOn);
        }

        [Fact]
        public void ShouldCreateProductPolicyTemplateResourceFromCreatorConfigWithCorrectContent()
        {
            // arrange
            PolicyTemplateCreator policyTemplateCreator = PolicyTemplateCreatorFactory.GeneratePolicyTemplateCreator();
            DeploymentDefinition creatorConfig = new DeploymentDefinition() { Products = new List<ProductDeploymentDefinition>() };
            ProductDeploymentDefinition product = new ProductDeploymentDefinition()
            {
                DisplayName = "displayName",
                Description = "description",
                Terms = "terms",
                SubscriptionRequired = true,
                ApprovalRequired = true,
                SubscriptionsLimit = 1,
                State = "state",
                Policy = "http://someurl.com"

            };
            creatorConfig.Products.Add(product);
            string[] dependsOn = new string[] { "dependsOn" };

            // act
            PolicyTemplateResource policyTemplateResource = policyTemplateCreator.CreateProductPolicyTemplateResource(product, dependsOn);

            // assert
            Assert.Equal($"[concat(parameters('ApimServiceName'), '/{product.Name}/policy')]", policyTemplateResource.Name);
            Assert.Equal("rawxml-link", policyTemplateResource.properties.format);
            Assert.Equal(product.Policy, policyTemplateResource.properties.value);
            Assert.Equal(dependsOn, policyTemplateResource.DependsOn);
        }

        [Fact]
        public void ShouldCreateOperationPolicyTemplateResourceFromPairWithCorrectContent()
        {
            // arrange
            PolicyTemplateCreator policyTemplateCreator = PolicyTemplateCreatorFactory.GeneratePolicyTemplateCreator();
            KeyValuePair<string, OperationsDeploymentDefinition> policyPair = new KeyValuePair<string, OperationsDeploymentDefinition>("key", new OperationsDeploymentDefinition() { Policy = "http://someurl.com" });
            string apiName = "apiName";
            string[] dependsOn = new string[] { "dependsOn" };

            // act
            PolicyTemplateResource policyTemplateResource = policyTemplateCreator.CreateOperationPolicyTemplateResource(policyPair, apiName, dependsOn);

            // assert
            Assert.Equal($"[concat(parameters('ApimServiceName'), '/{apiName}/{policyPair.Key}/policy')]", policyTemplateResource.Name);
            Assert.Equal("rawxml-link", policyTemplateResource.properties.format);
            Assert.Equal(policyPair.Value.Policy, policyTemplateResource.properties.value);
            Assert.Equal(dependsOn, policyTemplateResource.DependsOn);
        }
    }
}
