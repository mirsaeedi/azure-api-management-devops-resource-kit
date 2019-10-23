using System.Collections.Generic;
using Xunit;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Test
{
    public class MasterTemplateCreatorTests
    {
        [Fact]
        public void ShouldCreateCorrectNumberOfDeploymentResources()
        {
            // arrange
            CreatorConfig creatorConfig = new CreatorConfig() { ApimServiceName = "apimService", Linked = true };
            MasterTemplateCreator masterTemplateCreator = new MasterTemplateCreator();
            Template apiVersionSetsTemplate = new Template();
            Template globalServicePolicyTemplate = new Template();
            Template productsTemplate = new Template();
            Template loggersTemplate = new Template();
            List<LinkedMasterTemplateAPIInformation> apiInfoList = new List<LinkedMasterTemplateAPIInformation>() { new LinkedMasterTemplateAPIInformation() { name = "api", isSplit = true } };
            FileNameGenerator fileNameGenerator = new FileNameGenerator();
            FileNames creatorFileNames = fileNameGenerator.GenerateFileNames(creatorConfig.ApimServiceName);

            // should create 6 resources (globalServicePolicy, apiVersionSet, product, logger, both api templates)
            int count = 6;

            // act
            Template masterTemplate = masterTemplateCreator.CreateLinkedMasterTemplate(creatorConfig, globalServicePolicyTemplate, apiVersionSetsTemplate, productsTemplate, loggersTemplate, null, null, apiInfoList, creatorFileNames, creatorConfig.ApimServiceName, fileNameGenerator);

            // assert
            Assert.Equal(count, masterTemplate.resources.Length);
        }

        [Fact]
        public void ShouldCreateCorrectNumberOfParameterValuesWhenLinked()
        {
            // arrange
            MasterTemplateCreator masterTemplateCreator = new MasterTemplateCreator();
            CreatorConfig creatorConfig = new CreatorConfig()
            {
                ApimServiceName = "apimServiceName",
                Linked = true,
                LinkedTemplatesBaseUrl = "linkedTemplatesBaseUrl"
            };
            // linked templates result in 2 values
            int count = 2;

            // act
            Template masterTemplate = masterTemplateCreator.CreateMasterTemplateParameterValues(creatorConfig);

            // assert
            Assert.Equal(count, masterTemplate.Parameters.Count);
        }

        [Fact]
        public void ShouldCreateCorrectNumberOfParametersWhenUnlinked()
        {
            // arrange
            CreatorConfig creatorConfig = new CreatorConfig() { ApimServiceName = "apimService", Linked = false };
            MasterTemplateCreator masterTemplateCreator = new MasterTemplateCreator();
            // unlinked templates result in 1 value
            int count = 1;

            // act
            Dictionary<string, TemplateParameterProperties> masterTemplateParameters = masterTemplateCreator.CreateMasterTemplateParameters(creatorConfig);

            // assert
            Assert.Equal(count, masterTemplateParameters.Keys.Count);
        }

        [Fact]
        public void ShouldCreateLinkedMasterTemplateResourceFromValues()
        {
            // arrange
            MasterTemplateCreator masterTemplateCreator = new MasterTemplateCreator();
            string name = "name";
            string uriLink = "uriLink";
            string[] dependsOn = new string[] { "dependsOn" };

            // act
            MasterTemplateResource masterTemplateResource = masterTemplateCreator.CreateLinkedMasterTemplateResource(name, uriLink, dependsOn);

            // assert
            Assert.Equal(name, masterTemplateResource.Name);
            Assert.Equal(uriLink, masterTemplateResource.properties.templateLink.uri);
            Assert.Equal(dependsOn, masterTemplateResource.DependsOn);
        }

        [Fact]
        public void ShouldCreateCorrectLinkedUri()
        {
            // arrange
            MasterTemplateCreator masterTemplateCreator = new MasterTemplateCreator();
            CreatorConfig creatorConfig = new CreatorConfig() { ApimServiceName = "apimService", Linked = true, LinkedTemplatesBaseUrl = "http://someurl.com", LinkedTemplatesUrlQueryString = "?param=1" };
            string apiVersionSetFileName = "/versionSet1-apiVersionSets.template.json";

            // act
            string linkedResourceUri = masterTemplateCreator.GenerateLinkedTemplateUri(creatorConfig, apiVersionSetFileName);

            // assert
            Assert.Equal($"[concat(parameters('LinkedTemplatesBaseUrl'), '{apiVersionSetFileName}', parameters('LinkedTemplatesUrlQueryString'))]", linkedResourceUri);
        }
    }
}
