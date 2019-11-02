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
        private PolicyTemplateCreator _policyTemplateCreator;
        private APIVersionSetTemplateCreator _apiVersionSetTemplateCreator;
        private BackendTemplateCreator _backendTemplateCreator;
        private LoggerTemplateCreator _loggerTemplateCreator;
        private AuthorizationServerTemplateCreator _authorizationServerTemplateCreator;
        private ProductTemplateCreator _productsTemplateCreator;
        private FileReader _fileReader;

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
        public async Task<Template> Create(CreatorConfig creatorConfig)
        {
            var fileNameGenerator = new FileNameGenerator(creatorConfig.PrefixFileName, creatorConfig.MasterTemplateName);
            var fileNames = fileNameGenerator.GenerateFileNames();
            var fileReader = new FileReader();

            // create empty template
            Template masterTemplate = EmptyTemplate;

            // add parameters
            masterTemplate.Parameters = this.CreateMasterTemplateParameters(creatorConfig);

            // add deployment resources that links to all resource files
            var resources = new List<TemplateResource>();

            if(creatorConfig.Policy!=null)
                await CreateResource(resources, creatorConfig, fileNames.globalServicePolicy, "globalServicePolicyTemplate");
            
            if(creatorConfig.ApiVersionSets!=null)
                await CreateResource(resources, creatorConfig, fileNames.apiVersionSets, "versionSetTemplate");

            if (creatorConfig.Loggers != null)
                await CreateResource(resources, creatorConfig, fileNames.loggers, "loggersTemplate");

            if (creatorConfig.Backends != null)
                await CreateResource(resources, creatorConfig, fileNames.backends, "backendsTemplate");

            if (creatorConfig.AuthorizationServers != null)
                await CreateResource(resources, creatorConfig, fileNames.authorizationServers, "authorizationServersTemplate");

            if (creatorConfig.Products != null)
                await CreateResource(resources, creatorConfig, fileNames.products, "productsTemplate");

            // each api has an associated api info class that determines whether the api is split and its dependencies on other resources
            
            for(var i=0;i<creatorConfig.Apis.Count;i++)
            {
                var api = creatorConfig.Apis[i];

                var apiInfo = new LinkedMasterTemplateAPIInformation()
                {
                    Name = api.name,
                    IsSplit = true,
                    DependsOnGlobalServicePolicies = creatorConfig.Policy != null,
                    DependsOnVersionSets = api.apiVersionSetId != null,
                    DependsOnProducts = api.products != null,
                    DependsOnLoggers = await api.IsDependOnLogger(fileReader),
                    DependsOnAuthorizationServers = api.authenticationSettings != null && api.authenticationSettings.OAuth2 != null && api.authenticationSettings.OAuth2.AuthorizationServerId != null,
                    DependsOnBackends = await api.IsDependOnBackend(fileReader)
                };

                // add a deployment resource for both api template files
                string originalAPIName = fileNameGenerator.GenerateOriginalAPIName(apiInfo.Name);
                string initialAPIDeploymentResourceName = $"{originalAPIName}-InitialAPITemplate";
                string subsequentAPIDeploymentResourceName = $"{originalAPIName}-SubsequentAPITemplate";

                string initialAPIFileName = fileNameGenerator.GenerateCreatorAPIFileName(apiInfo.Name, apiInfo.IsSplit, true, creatorConfig.ApimServiceName);
                string initialAPIUri = GenerateLinkedTemplateUri(creatorConfig, initialAPIFileName);
                var initialAPIDependsOn = new List<string>(CreateAPIResourceDependencies(creatorConfig, apiInfo));

                if (i > 0)
                {
                    var previousApi = creatorConfig.Apis[i - 1];
                    initialAPIDependsOn.Add($"[resourceId('Microsoft.Resources/deployments', '{previousApi.name}-SubsequentAPITemplate')]");
                }

                resources.Add(this.CreateLinkedMasterTemplateResource(initialAPIDeploymentResourceName, initialAPIUri, initialAPIDependsOn.ToArray()));

                string subsequentAPIFileName = fileNameGenerator.GenerateCreatorAPIFileName(apiInfo.Name, apiInfo.IsSplit, false, creatorConfig.ApimServiceName);
                string subsequentAPIUri = GenerateLinkedTemplateUri(creatorConfig, subsequentAPIFileName);
                string[] subsequentAPIDependsOn = new string[] { $"[resourceId('Microsoft.Resources/deployments', '{initialAPIDeploymentResourceName}')]" };
                resources.Add(this.CreateLinkedMasterTemplateResource(subsequentAPIDeploymentResourceName, subsequentAPIUri, subsequentAPIDependsOn));
            }

            masterTemplate.Resources = resources.ToArray();
            return masterTemplate;
        }

        private async Task CreateResource(List<TemplateResource> resources, CreatorConfig creatorConfig, string fileName, string templateResourceName)
        {
            string linkedUri = GenerateLinkedTemplateUri(creatorConfig, fileName);

            var masterTemplateResource = CreateLinkedMasterTemplateResource(templateResourceName, linkedUri, new string[] { });

            resources.Add(masterTemplateResource);
        }

        public string[] CreateAPIResourceDependencies(CreatorConfig creatorConfig, LinkedMasterTemplateAPIInformation apiInfo)
        {
            List<string> apiDependsOn = new List<string>();

            if (creatorConfig.Policy!=null && apiInfo.DependsOnGlobalServicePolicies == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'globalServicePolicyTemplate')]");
            }
            if (creatorConfig.ApiVersionSets != null && apiInfo.DependsOnVersionSets == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'versionSetTemplate')]");
            }
            if (creatorConfig.Products != null && apiInfo.DependsOnProducts == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'productsTemplate')]");
            }
            if (creatorConfig.Loggers != null && apiInfo.DependsOnLoggers == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'loggersTemplate')]");
            }
            if (creatorConfig.Backends != null && apiInfo.DependsOnBackends == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'backendsTemplate')]");
            }
            if (creatorConfig.AuthorizationServers != null && apiInfo.DependsOnAuthorizationServers == true)
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
            // TODO
            return creatorConfig.LinkedTemplatesUrlQueryString != null ? $"[concat(parameters('LinkedTemplatesBaseUrl'), '/{fileName}', parameters('LinkedTemplatesUrlQueryString'))]" : $"[concat(parameters('LinkedTemplatesBaseUrl'), '/{fileName}')]";
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
