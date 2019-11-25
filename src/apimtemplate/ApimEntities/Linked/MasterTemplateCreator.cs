using System.Collections.Generic;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using System;
using Apim.DevOps.Toolkit.ArmTemplates;
using System.Linq;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class MasterTemplateCreator : TemplateCreator, ITemplateCreator 
    {
		private readonly NestedTemplateName _nestedTemplateName;
		private readonly TemplateFileName _templateFileNames;

        public MasterTemplateCreator(TemplateFileName templateFileNames)
        {
			_nestedTemplateName = new NestedTemplateName();
			_templateFileNames = templateFileNames;

		}
        public async Task<Template> Create(DeploymentDefinition creatorConfig)
		{
			Template masterTemplate = EmptyTemplate;
			masterTemplate.Parameters = this.CreateMasterTemplateParameters(creatorConfig);

			var resources = new List<TemplateResource>();

			await CreateResources(creatorConfig, resources);

			masterTemplate.Resources = resources.ToArray();
			return masterTemplate;
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


		private async Task CreateResources(DeploymentDefinition creatorConfig, List<TemplateResource> resources)
		{
			CreateGlobalPolicyResource(creatorConfig, resources);

			CreateApiVersionSetResource(creatorConfig, resources);

			CreateLoggerResource(creatorConfig, resources);

			CreateBackendResource(creatorConfig, resources);

			CreateAuthorizationServerResource(creatorConfig, resources);

			CreateProductResource(creatorConfig, resources);

			CreateTagResource(creatorConfig, resources);

			CreateUserResource(creatorConfig, resources);

			CreateSubscriptionResource(creatorConfig, resources);

			CreateCertificateResource(creatorConfig, resources);

			await CreateApiResource(creatorConfig, resources);
		}

		private void CreateGlobalPolicyResource(DeploymentDefinition creatorConfig, List<TemplateResource> resources)
		{
			if (creatorConfig.Policy != null)
				CreateResource(resources, creatorConfig, _templateFileNames.GlobalServicePolicy(), _nestedTemplateName.GlobalServicePolicy());
		}

		private void CreateApiVersionSetResource(DeploymentDefinition creatorConfig, List<TemplateResource> resources)
		{
			if (creatorConfig.ApiVersionSets != null)
				CreateResource(resources, creatorConfig, _templateFileNames.ApiVersionSets(), _nestedTemplateName.ApiVersionSets());
		}

		private void CreateLoggerResource(DeploymentDefinition creatorConfig, List<TemplateResource> resources)
		{
			if (creatorConfig.Loggers != null)
				CreateResource(resources, creatorConfig, _templateFileNames.Loggers(), _nestedTemplateName.Loggers());
		}

		private void CreateBackendResource(DeploymentDefinition creatorConfig, List<TemplateResource> resources)
		{
			if (creatorConfig.Backends != null)
				CreateResource(resources, creatorConfig, _templateFileNames.Backends(), _nestedTemplateName.Backends());
		}

		private void CreateAuthorizationServerResource(DeploymentDefinition creatorConfig, List<TemplateResource> resources)
		{
			if (creatorConfig.AuthorizationServers != null)

				CreateResource(resources, creatorConfig, _templateFileNames.AuthorizationServers(), _nestedTemplateName.AuthorizationServers());
		}

		private void CreateProductResource(DeploymentDefinition creatorConfig, List<TemplateResource> resources)
		{
			if (creatorConfig.Products != null)
			{
				var dependencies = new List<string>();

				if (creatorConfig.Tags != null && creatorConfig.Products.Any(q => q.IsDependOnTags()))
				{
					dependencies.Add(DependsOn(_nestedTemplateName.Tags()));
				}

				if (creatorConfig.Certificates != null)
				{
					dependencies.Add(DependsOn(_nestedTemplateName.Certificates()));
				}

				CreateResource(resources, creatorConfig, _templateFileNames.Products(), _nestedTemplateName.Products(),dependencies);
			}
		}

		private void CreateTagResource(DeploymentDefinition creatorConfig, List<TemplateResource> resources)
		{
			if (creatorConfig.Tags != null)
				CreateResource(resources, creatorConfig, _templateFileNames.Tags(), _nestedTemplateName.Tags());
		}

		private void CreateUserResource(DeploymentDefinition creatorConfig, List<TemplateResource> resources)
		{
			if (creatorConfig.Users != null)
				CreateResource(resources, creatorConfig, _templateFileNames.Users(), _nestedTemplateName.Users());
		}

		private void CreateSubscriptionResource(DeploymentDefinition creatorConfig, List<TemplateResource> resources)
		{
			if (creatorConfig.Subscriptions != null)
			{
				var dependencies = new List<string>();

				if (creatorConfig.Products != null)
				{
					dependencies.Add(DependsOn(_nestedTemplateName.Products()));
				}

				if (creatorConfig.Users != null)
				{
					dependencies.Add(DependsOn(_nestedTemplateName.Users()));
				}

				foreach (var api in creatorConfig.Apis)
				{
					dependencies.Add(DependsOn(_nestedTemplateName.ApiInitial(api.Name)));
				}

				CreateResource(resources, creatorConfig, _templateFileNames.Subscriptions(), _nestedTemplateName.Subscriptions(), dependencies.ToArray());
			}
		}

		private void CreateCertificateResource(DeploymentDefinition creatorConfig, List<TemplateResource> resources)
		{
			if (creatorConfig.Certificates != null)
				CreateResource(resources, creatorConfig, _templateFileNames.Certificates(), _nestedTemplateName.Certificates());
		}

		private async Task CreateApiResource(DeploymentDefinition creatorConfig, List<TemplateResource> resources)
		{
			for (var i = 0; i < creatorConfig.Apis.Count; i++)
			{
				var api = creatorConfig.Apis[i];

				var initialAPIDependsOn = new List<string>(await CreateApiResourceDependencies(creatorConfig, api));

				if (i > 0)
				{
					var previousApi = creatorConfig.Apis[i - 1];
					initialAPIDependsOn.Add(DependsOn(_nestedTemplateName.ApiSubsequent(previousApi.Name)));
				}

				CreateResource(resources, creatorConfig, _templateFileNames.ApiInitial(api.Name), _nestedTemplateName.ApiInitial(api.Name), initialAPIDependsOn);
				CreateResource(resources, creatorConfig, _templateFileNames.ApiSubsequent(api.Name), _nestedTemplateName.ApiSubsequent(api.Name), 
					new []{DependsOn(_nestedTemplateName.ApiInitial(api.Name))});
			}
		}

		private string DependsOn(string nestedTemplateName)
		{
			return $"[resourceId('Microsoft.Resources/deployments', '{nestedTemplateName}')]";
		}

		private void CreateResource(List<TemplateResource> resources, DeploymentDefinition creatorConfig, string fileName, string templateResourceName, IEnumerable<string> dependencies = null)
        {
            string linkedUri = GenerateLinkedTemplateUri(creatorConfig, fileName);

            var masterTemplateResource = CreateLinkedMasterTemplateResource(templateResourceName, linkedUri, dependencies ?? new string[0]);

            resources.Add(masterTemplateResource);
        }

        private async Task<string[]> CreateApiResourceDependencies(DeploymentDefinition creatorConfig, ApiDeploymentDefinition api)
        {
			var fileReader = new FileReader();

			List<string> apiDependsOn = new List<string>();

            if (api.IsDependOnGlobalServicePolicies(creatorConfig))
            {
                apiDependsOn.Add(DependsOn(_nestedTemplateName.GlobalServicePolicy()));
            }

            if (api.IsDependOnApiVersionSet())
            {
                apiDependsOn.Add(DependsOn(_nestedTemplateName.ApiVersionSets()));
            }

            if (api.IsDependOnProducts() && creatorConfig.Products?.Count>0)
            {
                apiDependsOn.Add(DependsOn(_nestedTemplateName.Products()));
            }

			if (api.IsDependOnTags() && creatorConfig.Tags?.Count > 0)
			{
				apiDependsOn.Add(DependsOn(_nestedTemplateName.Tags()));
			}

			if (await api.IsDependOnLogger(fileReader) && creatorConfig.Loggers != null)
            {
                apiDependsOn.Add(DependsOn(_nestedTemplateName.Loggers()));
            }

            if (await api.IsDependOnBackend(fileReader) && creatorConfig.Backends!=null)
            {
                apiDependsOn.Add(DependsOn(_nestedTemplateName.Backends()));
            }

            if (api.IsDependOnAuthorizationServers())
            {
                apiDependsOn.Add(DependsOn(_nestedTemplateName.AuthorizationServers()));
            }

			if (creatorConfig.Certificates != null)
			{
				apiDependsOn.Add(DependsOn(_nestedTemplateName.Certificates()));
			}

			return apiDependsOn.ToArray();
        }

        private MasterTemplateResource CreateLinkedMasterTemplateResource(string name, string uriLink, IEnumerable<string> dependsOn)
        {
            var masterTemplateResource = new MasterTemplateResource()
            {
                Name = name,
                ApiVersion = GlobalConstants.LinkedAPIVersion,
                Properties = new LinkedProperties()
                {
                    Mode = GlobalConstants.IncrementalArmDeployment,
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
                DependsOn = dependsOn.ToArray()
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

        private string GenerateLinkedTemplateUri(DeploymentDefinition creatorConfig, string fileName)
        {
            // TODO
            return creatorConfig.LinkedTemplatesUrlQueryString != null ? $"[concat(parameters('LinkedTemplatesBaseUrl'), '/{fileName}', parameters('LinkedTemplatesUrlQueryString'))]" : $"[concat(parameters('LinkedTemplatesBaseUrl'), '/{fileName}')]";
        }
    }
}
