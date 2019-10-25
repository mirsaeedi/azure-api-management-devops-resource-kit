using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using Apim.DevOps.Toolkit.Extensions;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class ApiTemplateCreator : TemplateCreator
    {
        private PolicyTemplateCreator _policyTemplateCreator;
        private ProductAPITemplateCreator _productAPITemplateCreator;
        private DiagnosticTemplateCreator _diagnosticTemplateCreator;
        private ReleaseTemplateCreator _releaseTemplateCreator;

        public ApiTemplateCreator()
        {
            _policyTemplateCreator = new PolicyTemplateCreator();
            _productAPITemplateCreator = new ProductAPITemplateCreator() ;
            _diagnosticTemplateCreator = new DiagnosticTemplateCreator();
            _releaseTemplateCreator = new ReleaseTemplateCreator();
        }

        public async Task<List<Template>> CreateAPITemplatesAsync(ApiConfiguration api)
        {
            // update api name if necessary (apiRevision > 1 and isCurrent = true) 
            if (int.TryParse(api.apiRevision, out var revisionNumber))
            {
                if (revisionNumber > 1 && api.isCurrent == true)
                {
                    api.name += $";rev={revisionNumber}";
                }
            }

            List<Template> apiTemplates = new List<Template>();
            apiTemplates.Add(await CreateApiTemplateAsync(api, true));
            apiTemplates.Add(await CreateApiTemplateAsync(api, false));

            return apiTemplates;
        }

        public async Task<Template> CreateApiTemplateAsync(ApiConfiguration api, bool isInitial)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();
            var apiTemplateResource = await CreateApiTemplateResourceAsync(api, isInitial);
            resources.Add(apiTemplateResource);

            // add the api child resources (api policies, diagnostics, etc) if this is the unified or subsequent template
            if (!isInitial)
            {
                resources.AddRange(CreateChildResourceTemplates(api));
            }

            template.resources = resources.ToArray();

            return await Task.FromResult(template);
        }

        public List<TemplateResource> CreateChildResourceTemplates(ApiConfiguration api)
        {
            var resources = new List<TemplateResource>();
            
            var dependsOn = new string[] { $"[resourceId('{ResourceType.Api}', parameters('ApimServiceName'), '{api.name}')]" };

            if (api.policy != null)
            {
                resources.Add(this._policyTemplateCreator.CreateAPIPolicyTemplateResource(api, dependsOn));
            }

            if (api.operations != null)
            {
                resources.AddRange(_policyTemplateCreator.CreateOperationPolicyTemplateResources(api, dependsOn));
            }

            if (api.products != null)
            {
                resources.AddRange(this._productAPITemplateCreator.CreateProductAPITemplateResources(api, dependsOn));
            }

            if (api.diagnostic != null)
            {
                resources.Add(this._diagnosticTemplateCreator.CreateAPIDiagnosticTemplateResource(api, dependsOn));
            }

            if (api.name.Contains(";rev"))
            {
                resources.Add(this._releaseTemplateCreator.CreateAPIReleaseTemplateResource(api, dependsOn));
            }

            return resources;
        }

        public async Task<ApiTemplateResource> CreateApiTemplateResourceAsync(ApiConfiguration api, bool isInitial)
        {
            ApiTemplateResource apiTemplateResource = new ApiTemplateResource()
            {
                Name = $"[concat(parameters('ApimServiceName'), '/{api.name}')]",
                Properties = new ApiTemplateProperties(),
                DependsOn = new string[] { }
            };

            if (isInitial)
            { 
                var openAPISpecReader = new OpenAPISpecReader(api.openApiSpec);
                
                apiTemplateResource.Properties.format = await openAPISpecReader.GetOpenApiFormat();
                apiTemplateResource.Properties.value = await openAPISpecReader.GetValue(); ;
                apiTemplateResource.Properties.path = api.suffix;
            }
            else
            {
                apiTemplateResource.Properties.apiVersion = api.apiVersion;
                apiTemplateResource.Properties.serviceUrl = api.serviceUrl;
                apiTemplateResource.Properties.type = api.type;
                apiTemplateResource.Properties.apiType = api.type; //todo
                apiTemplateResource.Properties.description = api.description;
                apiTemplateResource.Properties.subscriptionRequired = api.subscriptionRequired;
                apiTemplateResource.Properties.apiRevision = api.apiRevision;
                apiTemplateResource.Properties.apiRevisionDescription = api.apiRevisionDescription;
                apiTemplateResource.Properties.apiVersionDescription = api.apiVersionDescription;
                apiTemplateResource.Properties.AuthenticationSettings = api.authenticationSettings;
                apiTemplateResource.Properties.path = api.suffix;
                apiTemplateResource.Properties.isCurrent = api.isCurrent;
                apiTemplateResource.Properties.displayName = api.name;
                apiTemplateResource.Properties.subscriptionKeyParameterNames = api.subscriptionKeyParameterNames;
                apiTemplateResource.Properties.protocols = api.protocols.GetItem(new[] { "https"});
                
                if (api.apiVersionSetId != null)
                {
                    apiTemplateResource.Properties.ApiVersionSetId = $"[resourceId('{ResourceType.ApiVersionSet}', parameters('ApimServiceName'), '{api.apiVersionSetId}')]";
                }
                
                if (api.authenticationSettings != null && api.authenticationSettings.OAuth2 != null && api.authenticationSettings.OAuth2.AuthorizationServerId != null
                    && apiTemplateResource.Properties.AuthenticationSettings != null && apiTemplateResource.Properties.AuthenticationSettings.OAuth2 != null && apiTemplateResource.Properties.AuthenticationSettings.OAuth2.AuthorizationServerId != null)
                {
                    apiTemplateResource.Properties.AuthenticationSettings.OAuth2.AuthorizationServerId = api.authenticationSettings.OAuth2.AuthorizationServerId;
                }
            }

            return apiTemplateResource;
        }

    }
}
