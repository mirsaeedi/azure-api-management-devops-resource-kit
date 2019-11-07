using Xunit;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Test
{
    public class APIVersionSetTemplateCreatorTests
    {
        [Fact]
        public void ShouldCreateAPIVersionSetTemplateFromCreatorConfig()
        {
            // arrange
            APIVersionSetTemplateCreator apiVersionSetTemplateCreator = new APIVersionSetTemplateCreator();
            DeploymentDefinition creatorConfig = new DeploymentDefinition() { ApiVersionSets = new List<ApiVersionSetDeploymentDefinition>() };
            ApiVersionSetDeploymentDefinition apiVersionSet = new ApiVersionSetDeploymentDefinition()
            {
                Name = "id",
                Description = "description",
                DisplayName = "displayName",
                VersionHeaderName = "versionHeaderName",
                VersioningScheme = "versioningScheme",
                VersionQueryName = "versionQueryName"
            };
            creatorConfig.ApiVersionSets.Add(apiVersionSet);

            // act
            Template versionSetTemplate = apiVersionSetTemplateCreator.CreateAPIVersionSetTemplate(creatorConfig);
            APIVersionSetTemplateResource apiVersionSetTemplateResource = (APIVersionSetTemplateResource)versionSetTemplate.Resources[0];

            // assert
            Assert.Equal(apiVersionSet.Description, apiVersionSetTemplateResource.Properties.Description);
            Assert.Equal(apiVersionSet.DisplayName, apiVersionSetTemplateResource.Properties.DisplayName);
            Assert.Equal(apiVersionSet.VersionHeaderName, apiVersionSetTemplateResource.Properties.VersionHeaderName);
            Assert.Equal(apiVersionSet.VersioningScheme, apiVersionSetTemplateResource.Properties.VersioningScheme);
            Assert.Equal(apiVersionSet.VersionQueryName, apiVersionSetTemplateResource.Properties.VersionQueryName);
        }

        [Fact]
        public void ShouldUseDefaultResourceNameWithoutProvidedId()
        {
            // arrange
            APIVersionSetTemplateCreator apiVersionSetTemplateCreator = new APIVersionSetTemplateCreator();
            DeploymentDefinition creatorConfig = new DeploymentDefinition() { ApiVersionSets = new List<ApiVersionSetDeploymentDefinition>() };
            ApiVersionSetDeploymentDefinition apiVersionSet = new ApiVersionSetDeploymentDefinition();
            creatorConfig.ApiVersionSets.Add(apiVersionSet);

            // act
            Template versionSetTemplate = apiVersionSetTemplateCreator.CreateAPIVersionSetTemplate(creatorConfig);
            APIVersionSetTemplateResource apiVersionSetTemplateResource = (APIVersionSetTemplateResource)versionSetTemplate.Resources[0];

            // assert
            Assert.Equal("[concat(parameters('ApimServiceName'), '/versionset')]", apiVersionSetTemplateResource.Name);
        }

        [Fact]
        public void ShouldUseProvidedIdInResourceName()
        {
            // arrange
            APIVersionSetTemplateCreator apiVersionSetTemplateCreator = new APIVersionSetTemplateCreator();
            DeploymentDefinition creatorConfig = new DeploymentDefinition() { ApiVersionSets = new List<ApiVersionSetDeploymentDefinition>() };
            ApiVersionSetDeploymentDefinition apiVersionSet = new ApiVersionSetDeploymentDefinition()
            {
                Name = "id"
            };
            creatorConfig.ApiVersionSets.Add(apiVersionSet);

            // act
            Template versionSetTemplate = apiVersionSetTemplateCreator.CreateAPIVersionSetTemplate(creatorConfig);
            APIVersionSetTemplateResource apiVersionSetTemplateResource = (APIVersionSetTemplateResource)versionSetTemplate.Resources[0];

            // assert
            Assert.Equal($"[concat(parameters('ApimServiceName'), '/{apiVersionSet.Name}')]", apiVersionSetTemplateResource.Name);
        }
    }
}
