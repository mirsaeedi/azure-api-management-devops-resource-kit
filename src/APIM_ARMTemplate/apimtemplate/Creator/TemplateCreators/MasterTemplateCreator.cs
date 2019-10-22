using System.Collections.Generic;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class MasterTemplateCreator : TemplateCreator, ITemplateCreator 
    {
        private PolicyTemplateCreator _policyTemplateCreator;
        private APIVersionSetTemplateCreator _apiVersionSetTemplateCreator;
        private BackendTemplateCreator _backendTemplateCreator;
        private LoggerTemplateCreator _loggerTemplateCreator;
        private AuthorizationServerTemplateCreator _authorizationServerTemplateCreator;
        private ProductTemplateCreator _productsTemplate;
        private FileReader _fileReader;

        public MasterTemplateCreator(FileReader fileReader)
        {
            _fileReader = fileReader;
            var fileNameGenerator = new FileNameGenerator();

            _policyTemplateCreator = new PolicyTemplateCreator(_fileReader);
            _apiVersionSetTemplateCreator = new APIVersionSetTemplateCreator();
            _backendTemplateCreator = new BackendTemplateCreator();
            _loggerTemplateCreator = new LoggerTemplateCreator();
            _authorizationServerTemplateCreator = new AuthorizationServerTemplateCreator();
            _productsTemplate = new ProductTemplateCreator(_policyTemplateCreator);
            _apiVersionSetTemplateCreator = new APIVersionSetTemplateCreator();
            _apiVersionSetTemplateCreator = new APIVersionSetTemplateCreator();
        }
        public Template Create(CreatorConfig creatorConfig)
        {
            var apiInformation = new  List<LinkedMasterTemplateAPIInformation>();

            foreach (var api in creatorConfig.apis)
            {
                var apiTemplateCreator = new ApiTemplateCreator(_fileReader);

                apiInformation.Add(new LinkedMasterTemplateAPIInformation()
                {
                    Name = api.name,
                    IsSplit = api.IsSplitApi(),
                    DependsOnGlobalServicePolicies = creatorConfig.policy != null,
                    DependsOnVersionSets = api.apiVersionSetId != null,
                    DependsOnProducts = api.products != null,
                    DependsOnLoggers = await api.IsDependOnLogger(new FileReader()),
                    DependsOnAuthorizationServers = api.authenticationSettings != null && api.authenticationSettings.oAuth2 != null && api.authenticationSettings.oAuth2.authorizationServerId != null,
                    DependsOnBackends = await api.IsDependOnBackend(new FileReader())
                });
            }


            // create empty template
            Template masterTemplate = CreateEmptyTemplate();

            // add parameters
            masterTemplate.parameters = this.CreateMasterTemplateParameters(creatorConfig);

            // add deployment resources that links to all resource files
            List<TemplateResource> resources = new List<TemplateResource>();

            // globalServicePolicy
            if (globalServicePolicyTemplate != null)
            {
                string globalServicePolicyUri = GenerateLinkedTemplateUri(creatorConfig, fileNames.globalServicePolicy);
                resources.Add(this.CreateLinkedMasterTemplateResource("globalServicePolicyTemplate", globalServicePolicyUri, new string[] { }));
            }

            // apiVersionSet
            if (apiVersionSetTemplate != null)
            {
                string apiVersionSetUri = GenerateLinkedTemplateUri(creatorConfig, fileNames.apiVersionSets);
                resources.Add(this.CreateLinkedMasterTemplateResource("versionSetTemplate", apiVersionSetUri, new string[] { }));
            }

            // product
            if (productsTemplate != null)
            {
                string productsUri = GenerateLinkedTemplateUri(creatorConfig, fileNames.products);
                resources.Add(this.CreateLinkedMasterTemplateResource("productsTemplate", productsUri, new string[] { }));
            }

            // logger
            if (loggersTemplate != null)
            {
                string loggersUri = GenerateLinkedTemplateUri(creatorConfig, fileNames.loggers);
                resources.Add(this.CreateLinkedMasterTemplateResource("loggersTemplate", loggersUri, new string[] { }));
            }

            // backend
            if (backendsTemplate != null)
            {
                string backendsUri = GenerateLinkedTemplateUri(creatorConfig, fileNames.backends);
                resources.Add(this.CreateLinkedMasterTemplateResource("backendsTemplate", backendsUri, new string[] { }));
            }

            // authorizationServer
            if (authorizationServersTemplate != null)
            {
                string authorizationServersUri = GenerateLinkedTemplateUri(creatorConfig, fileNames.authorizationServers);
                resources.Add(this.CreateLinkedMasterTemplateResource("authorizationServersTemplate", authorizationServersUri, new string[] { }));
            }

            // each api has an associated api info class that determines whether the api is split and its dependencies on other resources
            foreach (LinkedMasterTemplateAPIInformation apiInfo in apiInformation)
            {
                if (apiInfo.IsSplit == true)
                {
                    // add a deployment resource for both api template files
                    string originalAPIName = fileNameGenerator.GenerateOriginalAPIName(apiInfo.Name);
                    string initialAPIDeploymentResourceName = $"{originalAPIName}-InitialAPITemplate";
                    string subsequentAPIDeploymentResourceName = $"{originalAPIName}-SubsequentAPITemplate";

                    string initialAPIFileName = fileNameGenerator.GenerateCreatorAPIFileName(apiInfo.Name, apiInfo.IsSplit, true, apimServiceName);
                    string initialAPIUri = GenerateLinkedTemplateUri(creatorConfig, initialAPIFileName);
                    string[] initialAPIDependsOn = CreateAPIResourceDependencies(globalServicePolicyTemplate, apiVersionSetTemplate, productsTemplate, loggersTemplate, backendsTemplate, authorizationServersTemplate, apiInfo);
                    resources.Add(this.CreateLinkedMasterTemplateResource(initialAPIDeploymentResourceName, initialAPIUri, initialAPIDependsOn));

                    string subsequentAPIFileName = fileNameGenerator.GenerateCreatorAPIFileName(apiInfo.Name, apiInfo.IsSplit, false, apimServiceName);
                    string subsequentAPIUri = GenerateLinkedTemplateUri(creatorConfig, subsequentAPIFileName);
                    string[] subsequentAPIDependsOn = new string[] { $"[resourceId('Microsoft.Resources/deployments', '{initialAPIDeploymentResourceName}')]" };
                    resources.Add(this.CreateLinkedMasterTemplateResource(subsequentAPIDeploymentResourceName, subsequentAPIUri, subsequentAPIDependsOn));
                }
                else
                {
                    // add a deployment resource for the unified api template file
                    string originalAPIName = fileNameGenerator.GenerateOriginalAPIName(apiInfo.Name);
                    string unifiedAPIDeploymentResourceName = $"{originalAPIName}-APITemplate";
                    string unifiedAPIFileName = fileNameGenerator.GenerateCreatorAPIFileName(apiInfo.Name, apiInfo.IsSplit, true, apimServiceName);
                    string unifiedAPIUri = GenerateLinkedTemplateUri(creatorConfig, unifiedAPIFileName);
                    string[] unifiedAPIDependsOn = CreateAPIResourceDependencies(globalServicePolicyTemplate, apiVersionSetTemplate, productsTemplate, loggersTemplate, backendsTemplate, authorizationServersTemplate, apiInfo);
                    resources.Add(this.CreateLinkedMasterTemplateResource(unifiedAPIDeploymentResourceName, unifiedAPIUri, unifiedAPIDependsOn));
                }
            }

            masterTemplate.resources = resources.ToArray();
            return masterTemplate;
        }

        public string[] CreateAPIResourceDependencies(Template globalServicePolicyTemplate,
            Template apiVersionSetTemplate,
            Template productsTemplate,
            Template loggersTemplate,
            Template backendsTemplate,
            Template authorizationServersTemplate,
            LinkedMasterTemplateAPIInformation apiInfo)
        {
            List<string> apiDependsOn = new List<string>();
            if (globalServicePolicyTemplate != null && apiInfo.DependsOnGlobalServicePolicies == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'globalServicePolicyTemplate')]");
            }
            if (apiVersionSetTemplate != null && apiInfo.DependsOnVersionSets == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'versionSetTemplate')]");
            }
            if (productsTemplate != null && apiInfo.DependsOnProducts == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'productsTemplate')]");
            }
            if (loggersTemplate != null && apiInfo.DependsOnLoggers == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'loggersTemplate')]");
            }
            if (backendsTemplate != null && apiInfo.DependsOnBackends == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'backendsTemplate')]");
            }
            if (authorizationServersTemplate != null && apiInfo.DependsOnAuthorizationServers == true)
            {
                apiDependsOn.Add("[resourceId('Microsoft.Resources/deployments', 'authorizationServersTemplate')]");
            }
            return apiDependsOn.ToArray();
        }

        public MasterTemplateResource CreateLinkedMasterTemplateResource(string name, string uriLink, string[] dependsOn)
        {
            // create deployment resource with provided arguments
            MasterTemplateResource masterTemplateResource = new MasterTemplateResource()
            {
                name = name,
                type = "Microsoft.Resources/deployments",
                apiVersion = GlobalConstants.LinkedAPIVersion,
                properties = new MasterTemplateProperties()
                {
                    mode = "Incremental",
                    templateLink = new MasterTemplateLink()
                    {
                        uri = uriLink,
                        contentVersion = "1.0.0.0"
                    },
                    parameters = new Dictionary<string, TemplateParameterProperties>
                    {
                        { "ApimServiceName", new TemplateParameterProperties(){ value = "[parameters('ApimServiceName')]" } }
                    }
                },
                dependsOn = dependsOn
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
            if (creatorConfig.linked == true)
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
                if (creatorConfig.linkedTemplatesUrlQueryString != null)
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
            // used to create the parameter values for use in parameters file
            // create empty template
            Template masterTemplate = CreateEmptyTemplate();

            // add parameters with value property
            Dictionary<string, TemplateParameterProperties> parameters = new Dictionary<string, TemplateParameterProperties>();
            TemplateParameterProperties apimServiceNameProperties = new TemplateParameterProperties()
            {
                value = creatorConfig.apimServiceName
            };
            parameters.Add("ApimServiceName", apimServiceNameProperties);
            if (creatorConfig.linked == true)
            {
                TemplateParameterProperties linkedTemplatesBaseUrlProperties = new TemplateParameterProperties()
                {
                    value = creatorConfig.linkedTemplatesBaseUrl
                };
                parameters.Add("LinkedTemplatesBaseUrl", linkedTemplatesBaseUrlProperties);
                if (creatorConfig.linkedTemplatesUrlQueryString != null)
                {
                    TemplateParameterProperties linkedTemplatesUrlQueryStringProperties = new TemplateParameterProperties()
                    {
                        value = creatorConfig.linkedTemplatesUrlQueryString
                    };
                    parameters.Add("LinkedTemplatesUrlQueryString", linkedTemplatesUrlQueryStringProperties);
                }
            }
            masterTemplate.parameters = parameters;
            return masterTemplate;
        }

        
        public string GenerateLinkedTemplateUri(CreatorConfig creatorConfig, string fileName)
        {
            return creatorConfig.linkedTemplatesUrlQueryString != null ? $"[concat(parameters('LinkedTemplatesBaseUrl'), '{fileName}', parameters('LinkedTemplatesUrlQueryString'))]" : $"[concat(parameters('LinkedTemplatesBaseUrl'), '{fileName}')]";
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
