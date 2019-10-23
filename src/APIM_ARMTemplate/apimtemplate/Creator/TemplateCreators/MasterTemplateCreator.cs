using System.Collections.Generic;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using System;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class MasterTemplateCreator : TemplateCreator, ITemplateCreator 
    {
        private PolicyTemplateCreator _policyTemplateCreator;
        private APIVersionSetTemplateCreator _apiVersionSetTemplateCreator;
        private BackendTemplateCreator _backendTemplateCreator;
        private LoggerTemplateCreator _loggerTemplateCreator;
        private AuthorizationServerTemplateCreator _authorizationServerTemplateCreator;
        private ProductTemplateCreator _productsTemplateCreator;
        private FileReader _fileReader;

        public MasterTemplateCreator(FileReader fileReader)
        {
            _fileReader = fileReader;
            _policyTemplateCreator = new PolicyTemplateCreator();
            _apiVersionSetTemplateCreator = new APIVersionSetTemplateCreator();
            _backendTemplateCreator = new BackendTemplateCreator();
            _loggerTemplateCreator = new LoggerTemplateCreator();
            _authorizationServerTemplateCreator = new AuthorizationServerTemplateCreator();
            _productsTemplateCreator = new ProductTemplateCreator();
            _apiVersionSetTemplateCreator = new APIVersionSetTemplateCreator();
        }
        public async Task<Template> Create(CreatorConfig creatorConfig)
        {
            var fileNameGenerator = new FileNameGenerator();
            var fileNames = fileNameGenerator.GenerateFileNames();
            var fileReader = new FileReader();

            // create empty template
            Template masterTemplate = EmptyTemplate;

            // add parameters
            masterTemplate.Parameters = this.CreateMasterTemplateParameters(creatorConfig);

            // add deployment resources that links to all resource files
            var resources = new List<TemplateResource>();

            CreateResource(resources, creatorConfig, _policyTemplateCreator, fileNames.globalServicePolicy, "globalServicePolicyTemplate");
            CreateResource(resources, creatorConfig, _apiVersionSetTemplateCreator, fileNames.apiVersionSets, "versionSetTemplate");
            CreateResource(resources, creatorConfig, _loggerTemplateCreator, fileNames.loggers, "loggersTemplate");
            CreateResource(resources, creatorConfig, _backendTemplateCreator, fileNames.backends, "backendsTemplate");
            CreateResource(resources, creatorConfig, _authorizationServerTemplateCreator, fileNames.authorizationServers, "authorizationServersTemplate");
            CreateResource(resources, creatorConfig, _productsTemplateCreator, fileNames.products, "productsTemplate");

            // each api has an associated api info class that determines whether the api is split and its dependencies on other resources
            foreach (var api in creatorConfig.Apis)
            {
                var apiInfo = new LinkedMasterTemplateAPIInformation()
                {
                    Name = api.name,
                    IsSplit = api.IsSplitApi(),
                    DependsOnGlobalServicePolicies = creatorConfig.Policy != null,
                    DependsOnVersionSets = api.apiVersionSetId != null,
                    DependsOnProducts = api.products != null,
                    DependsOnLoggers = await api.IsDependOnLogger(fileReader),
                    DependsOnAuthorizationServers = api.authenticationSettings != null && api.authenticationSettings.OAuth2 != null && api.authenticationSettings.OAuth2.AuthorizationServerId != null,
                    DependsOnBackends = await api.IsDependOnBackend(fileReader)
                };

                if (apiInfo.IsSplit == true)
                {
                    // add a deployment resource for both api template files
                    string originalAPIName = fileNameGenerator.GenerateOriginalAPIName(apiInfo.Name);
                    string initialAPIDeploymentResourceName = $"{originalAPIName}-InitialAPITemplate";
                    string subsequentAPIDeploymentResourceName = $"{originalAPIName}-SubsequentAPITemplate";

                    string initialAPIFileName = fileNameGenerator.GenerateCreatorAPIFileName(apiInfo.Name, apiInfo.IsSplit, true, creatorConfig.ApimServiceName);
                    string initialAPIUri = GenerateLinkedTemplateUri(creatorConfig, initialAPIFileName);
                    string[] initialAPIDependsOn = CreateAPIResourceDependencies(apiInfo);
                    resources.Add(this.CreateLinkedMasterTemplateResource(initialAPIDeploymentResourceName, initialAPIUri, initialAPIDependsOn));

                    string subsequentAPIFileName = fileNameGenerator.GenerateCreatorAPIFileName(apiInfo.Name, apiInfo.IsSplit, false, creatorConfig.ApimServiceName);
                    string subsequentAPIUri = GenerateLinkedTemplateUri(creatorConfig, subsequentAPIFileName);
                    string[] subsequentAPIDependsOn = new string[] { $"[resourceId('Microsoft.Resources/deployments', '{initialAPIDeploymentResourceName}')]" };
                    resources.Add(this.CreateLinkedMasterTemplateResource(subsequentAPIDeploymentResourceName, subsequentAPIUri, subsequentAPIDependsOn));
                }
                else
                {
                    // add a deployment resource for the unified api template file
                    string originalAPIName = fileNameGenerator.GenerateOriginalAPIName(apiInfo.Name);
                    string unifiedAPIDeploymentResourceName = $"{originalAPIName}-APITemplate";
                    string unifiedAPIFileName = fileNameGenerator.GenerateCreatorAPIFileName(apiInfo.Name, apiInfo.IsSplit, true, creatorConfig.ApimServiceName);
                    string unifiedAPIUri = GenerateLinkedTemplateUri(creatorConfig, unifiedAPIFileName);
                    string[] unifiedAPIDependsOn = CreateAPIResourceDependencies(apiInfo);
                    resources.Add(this.CreateLinkedMasterTemplateResource(unifiedAPIDeploymentResourceName, unifiedAPIUri, unifiedAPIDependsOn));
                }
            }

            masterTemplate.resources = resources.ToArray();
            return masterTemplate;
        }

        private void CreateResource(List<TemplateResource> resources, CreatorConfig creatorConfig, ITemplateCreator templateCreator, string fileName, string templateResourceName)
        {
            var template = templateCreator.Create(creatorConfig);

            if (template != null)
            {
                string linkedUri = GenerateLinkedTemplateUri(creatorConfig, fileName);

                var masterTemplateResource = CreateLinkedMasterTemplateResource(templateResourceName, linkedUri, new string[] { });

                resources.Add(masterTemplateResource);
            }
        }

        public string[] CreateAPIResourceDependencies(LinkedMasterTemplateAPIInformation apiInfo)
        {
            List<string> apiDependsOn = new List<string>();

            if (apiInfo.DependsOnGlobalServicePolicies == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'globalServicePolicyTemplate')]");
            }
            if (apiInfo.DependsOnVersionSets == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'versionSetTemplate')]");
            }
            if (apiInfo.DependsOnProducts == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'productsTemplate')]");
            }
            if (apiInfo.DependsOnLoggers == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'loggersTemplate')]");
            }
            if (apiInfo.DependsOnBackends == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'backendsTemplate')]");
            }
            if (apiInfo.DependsOnAuthorizationServers == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'authorizationServersTemplate')]");
            }

            return apiDependsOn.ToArray();
        }

        public MasterTemplateResource CreateLinkedMasterTemplateResource(string name, string uriLink, string[] dependsOn)
        {
            // create deployment resource with provided arguments
            var masterTemplateResource = new MasterTemplateResource()
            {
                Name = name,
                ApiVersion = GlobalConstants.LinkedAPIVersion,
                Properties = new MasterTemplateProperties()
                {
                    Mode = "Incremental",
                    TemplateLink = new MasterTemplateLink()
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

        public Dictionary<string, TemplateParameterProperties> CreateMasterTemplateParameters(CreatorConfig creatorConfig)
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
            if (creatorConfig.Linked == true)
            {
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
            }
            return parameters;
        }

        public Template CreateMasterTemplateParameterValues(CreatorConfig creatorConfig)
        {
            var masterTemplate = EmptyTemplate;

            // add parameters with value property
            Dictionary<string, TemplateParameterProperties> parameters = new Dictionary<string, TemplateParameterProperties>();
            TemplateParameterProperties apimServiceNameProperties = new TemplateParameterProperties()
            {
                value = creatorConfig.ApimServiceName
            };
            parameters.Add("ApimServiceName", apimServiceNameProperties);
            if (creatorConfig.Linked == true)
            {
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
            }
            masterTemplate.Parameters = parameters;
            return masterTemplate;
        }

        
        public string GenerateLinkedTemplateUri(CreatorConfig creatorConfig, string fileName)
        {
            return creatorConfig.LinkedTemplatesUrlQueryString != null ? $"[concat(parameters('LinkedTemplatesBaseUrl'), '{fileName}', parameters('LinkedTemplatesUrlQueryString'))]" : $"[concat(parameters('LinkedTemplatesBaseUrl'), '{fileName}')]";
        }
    }

    public class LinkedMasterTemplateAPIInformation
    {
        public string Name { get; set; }
        public bool IsSplit { get; set; }
        public bool DependsOnGlobalServicePolicies { get; set; }
        public bool DependsOnVersionSets { get; set; }
        public bool DependsOnProducts { get; set; }
        public bool DependsOnLoggers { get; set; }
        public bool DependsOnBackends { get; set; }
        public bool DependsOnAuthorizationServers { get; set; }
    }

}
