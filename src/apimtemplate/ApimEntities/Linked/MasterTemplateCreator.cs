using System.Collections.Generic;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using System;
using Apim.DevOps.Toolkit.ArmTemplates;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class MasterTemplateCreator : TemplateCreator, ITemplateCreator 
    {
        private readonly PolicyTemplateCreator _policyTemplateCreator;
        private readonly APIVersionSetTemplateCreator _apiVersionSetTemplateCreator;
        private readonly BackendTemplateCreator _backendTemplateCreator;
        private readonly LoggerTemplateCreator _loggerTemplateCreator;
        private readonly AuthorizationServerTemplateCreator _authorizationServerTemplateCreator;
        private readonly ProductTemplateCreator _productsTemplateCreator;
        private readonly FileReader _fileReader;

        public MasterTemplateCreator()
        {
            _fileReader = new FileReader();
            _policyTemplateCreator = new PolicyTemplateCreator();
            _apiVersionSetTemplateCreator = new APIVersionSetTemplateCreator();
            _backendTemplateCreator = new BackendTemplateCreator();
            _loggerTemplateCreator = new LoggerTemplateCreator();
            _authorizationServerTemplateCreator = new AuthorizationServerTemplateCreator();
            _productsTemplateCreator = new ProductTemplateCreator();
            _apiVersionSetTemplateCreator = new APIVersionSetTemplateCreator();
        }
        public async Task<Template> Create(DeploymentDefinition creatorConfig)
        {
            var fileNameGenerator = new FileNameGenerator(creatorConfig.PrefixFileName, creatorConfig.MasterTemplateName);
            var fileNames = fileNameGenerator.GenerateFileNames();

            // create empty template
            Template masterTemplate = EmptyTemplate;

            // add parameters
            masterTemplate.Parameters = this.CreateMasterTemplateParameters(creatorConfig);

            // add deployment resources that links to all resource files
            var resources = new List<TemplateResource>();

            if(creatorConfig.Policy!=null)
                await CreateResource(resources, creatorConfig, fileNames.GlobalServicePolicy, "globalServicePolicyTemplate");
            
            if(creatorConfig.ApiVersionSets!=null)
                await CreateResource(resources, creatorConfig, fileNames.ApiVersionSets, "versionSetTemplate");

            if (creatorConfig.Loggers != null)
                await CreateResource(resources, creatorConfig, fileNames.Loggers, "loggersTemplate");

            if (creatorConfig.Backends != null)
                await CreateResource(resources, creatorConfig, fileNames.Backends, "backendsTemplate");

            if (creatorConfig.AuthorizationServers != null)
                await CreateResource(resources, creatorConfig, fileNames.AuthorizationServers, "authorizationServersTemplate");

            if (creatorConfig.Products != null)
                await CreateResource(resources, creatorConfig, fileNames.Products, "productsTemplate");

			if (creatorConfig.Tags!= null)
				await CreateResource(resources, creatorConfig, fileNames.Tags, "tagsTemplate");

			// each api has an associated api info class that determines whether the api is split and its dependencies on other resources

			for (var i=0;i<creatorConfig.Apis.Count;i++)
            {
                var api = creatorConfig.Apis[i];

                // add a deployment resource for both api template files
                string originalAPIName = fileNameGenerator.GenerateOriginalAPIName(api.Name);
                string initialAPIDeploymentResourceName = $"{originalAPIName}-InitialAPITemplate";
                string subsequentAPIDeploymentResourceName = $"{originalAPIName}-SubsequentAPITemplate";

                string initialAPIFileName = fileNameGenerator.GenerateCreatorAPIFileName(api.Name, true, true, creatorConfig.ApimServiceName);
                string initialAPIUri = GenerateLinkedTemplateUri(creatorConfig, initialAPIFileName);
                var initialAPIDependsOn = new List<string>(await CreateAPIResourceDependencies(creatorConfig, api));

                if (i > 0)
                {
                    var previousApi = creatorConfig.Apis[i - 1];
                    initialAPIDependsOn.Add($"[resourceId('Microsoft.Resources/deployments', '{previousApi.Name}-SubsequentAPITemplate')]");
                }

                resources.Add(this.CreateLinkedMasterTemplateResource(initialAPIDeploymentResourceName, initialAPIUri, initialAPIDependsOn.ToArray()));

                string subsequentAPIFileName = fileNameGenerator.GenerateCreatorAPIFileName(api.Name, true, false, creatorConfig.ApimServiceName);
                string subsequentAPIUri = GenerateLinkedTemplateUri(creatorConfig, subsequentAPIFileName);
                string[] subsequentAPIDependsOn = new string[] { $"[resourceId('Microsoft.Resources/deployments', '{initialAPIDeploymentResourceName}')]" };
                resources.Add(this.CreateLinkedMasterTemplateResource(subsequentAPIDeploymentResourceName, subsequentAPIUri, subsequentAPIDependsOn));
            }

            masterTemplate.Resources = resources.ToArray();
            return masterTemplate;
        }

        private async Task CreateResource(List<TemplateResource> resources, DeploymentDefinition creatorConfig, string fileName, string templateResourceName)
        {
            string linkedUri = GenerateLinkedTemplateUri(creatorConfig, fileName);

            var masterTemplateResource = CreateLinkedMasterTemplateResource(templateResourceName, linkedUri, new string[] { });

            resources.Add(masterTemplateResource);
        }

        private async Task<string[]> CreateAPIResourceDependencies(DeploymentDefinition creatorConfig, ApiDeploymentDefinition api)
        {
			var fileReader = new FileReader();

			List<string> apiDependsOn = new List<string>();

            if (api.IsDependOnGlobalServicePolicies(creatorConfig))
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'globalServicePolicyTemplate')]");
            }

            if (api.IsDependOnApiVersionSet())
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'versionSetTemplate')]");
            }

            if (api.IsDependOnProducts() && creatorConfig.Products?.Count>0)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'productsTemplate')]");
            }

			if (api.IsDependOnTags())
			{
				apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'tagsTemplate')]");
			}

			if (await api.IsDependOnLogger(fileReader) && creatorConfig.Loggers != null)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'loggersTemplate')]");
            }

            if (await api.IsDependOnBackend(fileReader) && creatorConfig.Backends!=null)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'backendsTemplate')]");
            }

            if (api.IsDependOnAuthorizationServers())
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'authorizationServersTemplate')]");
            }

            return apiDependsOn.ToArray();
        }

        private MasterTemplateResource CreateLinkedMasterTemplateResource(string name, string uriLink, string[] dependsOn)
        {
            // create deployment resource with provided arguments
            var masterTemplateResource = new MasterTemplateResource()
            {
                Name = name,
                ApiVersion = GlobalConstants.LinkedAPIVersion,
                Properties = new LinkedProperties()
                {
                    Mode = "Incremental",
                    TemplateLink = new LinkedTemplateLink()
                    {
                        Uri = uriLink,
                        ContentVersion = "1.0.0.0"
                    },
                    Parameters = new Dictionary<string, TemplateParameterProperties>
                    {
                        { "ApimServiceName", new TemplateParameterProperties(){ value = "[parameters('ApimServiceName')]" } }
                    }
                },
                DependsOn = dependsOn
            };

            return masterTemplateResource;
        }

        private Dictionary<string, TemplateParameterProperties> CreateMasterTemplateParameters(DeploymentDefinition creatorConfig)
        {
            // used to create the parameter metatadata, etc (not value) for use in file with resources
            // add parameters with metatdata properties
            Dictionary<string, TemplateParameterProperties> parameters = new Dictionary<string, TemplateParameterProperties>();
            TemplateParameterProperties apimServiceNameProperties = new TemplateParameterProperties()
            {
                metadata = new TemplateParameterMetadata()
                {
                    description = "Name of the API Management"
                },
                type = "string"
            };
            parameters.Add("ApimServiceName", apimServiceNameProperties);
			// add remote location of template files for linked option

			TemplateParameterProperties linkedTemplatesBaseUrlProperties = new TemplateParameterProperties()
			{
				metadata = new TemplateParameterMetadata()
				{
					description = "Base URL of the repository"
				},
				type = "string"
			};
			parameters.Add("LinkedTemplatesBaseUrl", linkedTemplatesBaseUrlProperties);

			if (creatorConfig.LinkedTemplatesUrlQueryString != null)
			{
				TemplateParameterProperties linkedTemplatesUrlQueryStringProperties = new TemplateParameterProperties()
				{
					metadata = new TemplateParameterMetadata()
					{
						description = "Query string for the URL of the repository"
					},
					type = "string"
				};
				parameters.Add("LinkedTemplatesUrlQueryString", linkedTemplatesUrlQueryStringProperties);
			}

			return parameters;
        }

        public Template CreateMasterTemplateParameterValues(DeploymentDefinition creatorConfig)
        {
            var masterTemplate = EmptyTemplate;

            // add parameters with value property
            Dictionary<string, TemplateParameterProperties> parameters = new Dictionary<string, TemplateParameterProperties>();
            TemplateParameterProperties apimServiceNameProperties = new TemplateParameterProperties()
            {
                value = creatorConfig.ApimServiceName
            };
            parameters.Add("ApimServiceName", apimServiceNameProperties);

			TemplateParameterProperties linkedTemplatesBaseUrlProperties = new TemplateParameterProperties()
			{
				value = creatorConfig.LinkedTemplatesBaseUrl
			};
			parameters.Add("LinkedTemplatesBaseUrl", linkedTemplatesBaseUrlProperties);

			if (creatorConfig.LinkedTemplatesUrlQueryString != null)
			{
				TemplateParameterProperties linkedTemplatesUrlQueryStringProperties = new TemplateParameterProperties()
				{
					value = creatorConfig.LinkedTemplatesUrlQueryString
				};
				parameters.Add("LinkedTemplatesUrlQueryString", linkedTemplatesUrlQueryStringProperties);
			}


			masterTemplate.Parameters = parameters;
            return masterTemplate;
        }
        private string GenerateLinkedTemplateUri(DeploymentDefinition creatorConfig, string fileName)
        {
            // TODO
            return creatorConfig.LinkedTemplatesUrlQueryString != null ? $"[concat(parameters('LinkedTemplatesBaseUrl'), '/{fileName}', parameters('LinkedTemplatesUrlQueryString'))]" : $"[concat(parameters('LinkedTemplatesBaseUrl'), '/{fileName}')]";
        }
    }
}
