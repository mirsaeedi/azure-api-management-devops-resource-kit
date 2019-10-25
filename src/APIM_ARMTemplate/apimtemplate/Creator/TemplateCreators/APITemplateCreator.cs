using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using Apim.DevOps.Toolkit.Extensions;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class ApiTemplateCreator : TemplateCreator
    {
        private FileReader fileReader;
        private PolicyTemplateCreator _policyTemplateCreator;
        private ProductAPITemplateCreator _productAPITemplateCreator;
        private DiagnosticTemplateCreator _diagnosticTemplateCreator;
        private ReleaseTemplateCreator _releaseTemplateCreator;

        public ApiTemplateCreator(FileReader fileReader)
        {
            this.fileReader = fileReader;
            _policyTemplateCreator = new PolicyTemplateCreator();
            _productAPITemplateCreator = new ProductAPITemplateCreator() ;
            _diagnosticTemplateCreator = new DiagnosticTemplateCreator();
            _releaseTemplateCreator = new ReleaseTemplateCreator();
        }

        public async Task<List<Template>> CreateAPITemplatesAsync(ApiConfiguration api)
        {
            // determine if api needs to be split into multiple templates
            bool isSplit = api.IsSplitApi();

            // update api name if necessary (apiRevision > 1 and isCurrent = true) 
            int revisionNumber = 0;
            if (Int32.TryParse(api.apiRevision, out revisionNumber))
            {
                if (revisionNumber > 1 && api.isCurrent == true)
                {
                    string currentAPIName = api.name;
                    api.name += $";rev={revisionNumber}";
                }
            }

            List<Template> apiTemplates = new List<Template>();
            if (isSplit == true)
            {
                // create 2 templates, an initial template with metadata and a subsequent template with the swagger content
                apiTemplates.Add(await CreateAPITemplateAsync(api, isSplit, true));
                apiTemplates.Add(await CreateAPITemplateAsync(api, isSplit, false));
            }
            else
            {
                // create a unified template that includes both the metadata and swagger content 
                apiTemplates.Add(await CreateAPITemplateAsync(api, isSplit, false));
            }
            return apiTemplates;
        }

        public async Task<Template> CreateAPITemplateAsync(ApiConfiguration api, bool isSplit, bool isInitial)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();
            var apiTemplateResource = await CreateApiTemplateResourceAsync(api, isSplit, isInitial);
            resources.Add(apiTemplateResource);

            // add the api child resources (api policies, diagnostics, etc) if this is the unified or subsequent template
            if (!isSplit || !isInitial)
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

        public async Task<ApiTemplateResource> CreateApiTemplateResourceAsync(ApiConfiguration api, bool isSplit, bool isInitial)
        {
            // create api resource
            ApiTemplateResource apiTemplateResource = new ApiTemplateResource()
            {
                Name = $"[concat(parameters('ApimServiceName'), '/{api.name}')]",
                Properties = new ApiTemplateProperties(),
                DependsOn = new string[] { }
            };

            // add Properties depending on whether the template is the initial, subsequent, or unified 
            if (!isSplit || isInitial)
            { 
                var openAPISpecReader = new OpenAPISpecReader(api.openApiSpec);
                
                apiTemplateResource.Properties.format = await openAPISpecReader.GetOpenApiFormat();
                apiTemplateResource.Properties.value = await openAPISpecReader.GetValue(); ;
                apiTemplateResource.Properties.path = api.suffix;
            }
            if (!isSplit || !isInitial)
            {
                // add metadata Properties for initial and unified templates
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
                // set the version set id
                if (api.apiVersionSetId != null)
                {
                    // point to the supplied version set if the apiVersionSetId is provided
                    apiTemplateResource.Properties.ApiVersionSetId = $"[resourceId('{ResourceType.ApiVersionSet}', parameters('ApimServiceName'), '{api.apiVersionSetId}')]";
                }
                // set the authorization server id
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
