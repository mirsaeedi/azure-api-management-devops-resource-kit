using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using Apim.DevOps.Toolkit.Extensions;
using Apim.DevOps.Toolkit.ArmTemplates;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class ApiTemplateCreator : TemplateCreator
    {
		private ApiOperationPolicyTemplateCreator _apiOperationPolicyTemplateCreator;
		private ApiPolicyTemplateCreator _apiPolicyTemplateCreator;
        private ProductApiTemplateCreator _productApiTemplateCreator;
		private TagApiTemplateCreator _tagApiTemplateCreator;
		private DiagnosticTemplateCreator _diagnosticTemplateCreator;
        private ReleaseTemplateCreator _releaseTemplateCreator;
		private IEnumerable<ProductDeploymentDefinition> _products;

		public ApiTemplateCreator(IEnumerable<ProductDeploymentDefinition> products,IEnumerable<TagDeploymentDefinition> tags)
        {
			_apiOperationPolicyTemplateCreator = new ApiOperationPolicyTemplateCreator();
			_apiPolicyTemplateCreator = new ApiPolicyTemplateCreator();
            _productApiTemplateCreator = new ProductApiTemplateCreator(products);
			_tagApiTemplateCreator = new TagApiTemplateCreator(tags);
			_diagnosticTemplateCreator = new DiagnosticTemplateCreator();
            _releaseTemplateCreator = new ReleaseTemplateCreator();
		}

        public async Task<(Template InitialApiTemplate, Template SubsequentApiTemplate)> CreateApiTemplatesAsync(ApiDeploymentDefinition api)
        {
            // update api name if necessary (apiRevision > 1 and isCurrent = true) 
            if (int.TryParse(api.ApiRevision, out var revisionNumber))
            {
                if (revisionNumber > 1 && api.IsCurrent == true)
                {
                    api.Name += $";rev={revisionNumber}";
                }
            }

            var initial = await CreateApiTemplateAsync(api, true);
            var subsequent = await CreateApiTemplateAsync(api, false);

            return (initial,subsequent);
        }

        private async Task<Template> CreateApiTemplateAsync(ApiDeploymentDefinition api, bool isInitial)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();
            var apiTemplateResource = await CreateApiTemplateResourceAsync(api, isInitial);
            resources.Add(apiTemplateResource);

            if (!isInitial)
            {
                resources.AddRange(await CreateChildResourceTemplates(api));
            }

            template.Resources = resources.ToArray();

            return await Task.FromResult(template);
        }

        private async Task<List<TemplateResource>> CreateChildResourceTemplates(ApiDeploymentDefinition api)
        {
            var resources = new List<TemplateResource>();
            
            var dependsOn = new string[] { $"[resourceId('{ResourceType.Api}', parameters('ApimServiceName'), '{api.Name}')]" };

            if (api.Policy != null)
            {
                resources.Add(await _apiPolicyTemplateCreator.Create(api, dependsOn));
            }

            if (api.Operations != null)
            {
                resources.AddRange(await _apiOperationPolicyTemplateCreator.Create(api, dependsOn));
            }

            if (api.IsDependOnProducts())
            {
                resources.AddRange(this._productApiTemplateCreator.CreateProductApiTemplateResources(api, dependsOn));
            }

			if (api.IsDependOnTags())
			{
				resources.AddRange(this._tagApiTemplateCreator.CreateTagApiTemplateResources(api, dependsOn));
			}

			if (api.Diagnostic != null)
            {
                resources.Add(this._diagnosticTemplateCreator.CreateAPIDiagnosticTemplateResource(api, dependsOn));
            }

            if (api.Name.Contains(";rev"))
            {
                resources.Add(this._releaseTemplateCreator.CreateAPIReleaseTemplateResource(api, dependsOn));
            }

            return resources;
        }

        private async Task<ApiTemplateResource> CreateApiTemplateResourceAsync(ApiDeploymentDefinition api, bool isInitial)
        {
            ApiTemplateResource apiTemplateResource = new ApiTemplateResource()
            {
                Name = $"[concat(parameters('ApimServiceName'), '/{api.Name}')]",
                Properties = new ApiProperties(),
                DependsOn = new string[] { }
            };

            if (isInitial)
            { 
                var openAPISpecReader = new OpenAPISpecReader(api.OpenApiSpec);
                
                apiTemplateResource.Properties.Format = await openAPISpecReader.GetOpenApiFormat();
                apiTemplateResource.Properties.Value = await openAPISpecReader.GetValue(); ;
                apiTemplateResource.Properties.Path = api.Path;
            }
            else
            {
				var path = api.Path.Replace("//","/");
				if (path.StartsWith("/"))
					path = path.Substring(1);

				apiTemplateResource.Properties.ApiVersion = api.ApiVersion;
                apiTemplateResource.Properties.ServiceUrl = api.ServiceUrl;
                apiTemplateResource.Properties.Type = api.Type;
                apiTemplateResource.Properties.ApiType = api.ApiType; 
                apiTemplateResource.Properties.Description = api.Description;
                apiTemplateResource.Properties.SubscriptionRequired = api.SubscriptionRequired;
                apiTemplateResource.Properties.ApiRevision = api.ApiRevision;
                apiTemplateResource.Properties.ApiRevisionDescription = api.ApiRevisionDescription;
                apiTemplateResource.Properties.ApiVersionDescription = api.ApiVersionDescription;
                apiTemplateResource.Properties.AuthenticationSettings = api.AuthenticationSettings;
                apiTemplateResource.Properties.Path = path;
                apiTemplateResource.Properties.IsCurrent = api.IsCurrent;
                apiTemplateResource.Properties.DisplayName = api.DisplayName;
				apiTemplateResource.Properties.SubscriptionKeyParameterNames = api.SubscriptionKeyParameterNames;
                apiTemplateResource.Properties.Protocols = api.Protocols.GetItems(new[] { "https"});
                
                if (api.ApiVersionSetId != null)
                {
                    apiTemplateResource.Properties.ApiVersionSetId = $"[resourceId('{ResourceType.ApiVersionSet}', parameters('ApimServiceName'), '{api.ApiVersionSetId}')]";
                }
                
                if (api.AuthenticationSettings != null && api.AuthenticationSettings.OAuth2 != null && api.AuthenticationSettings.OAuth2.AuthorizationServerId != null
                    && apiTemplateResource.Properties.AuthenticationSettings != null && apiTemplateResource.Properties.AuthenticationSettings.OAuth2 != null && apiTemplateResource.Properties.AuthenticationSettings.OAuth2.AuthorizationServerId != null)
                {
                    apiTemplateResource.Properties.AuthenticationSettings.OAuth2.AuthorizationServerId = api.AuthenticationSettings.OAuth2.AuthorizationServerId;
                }
            }

            return apiTemplateResource;
        }

    }
}
