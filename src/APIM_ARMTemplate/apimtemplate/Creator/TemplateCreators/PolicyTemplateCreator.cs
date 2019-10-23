using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class PolicyTemplateCreator: TemplateCreator,ITemplateCreator
    {
        private FileReader _fileReader;

        public PolicyTemplateCreator()
        {
            _fileReader = new FileReader();
        }

        public async Task<Template> Create(CreatorConfig creatorConfig)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            string globalServicePolicy = creatorConfig.Policy;
            
            bool isUrl = Uri.TryCreate(globalServicePolicy, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            // create policy resource with properties
            var policyTemplateResource = new PolicyTemplateResource(ResourceType.GlobalServicePolicy)
            {
                Name = $"[concat(parameters('ApimServiceName'), '/policy')]",
                Properties = new PolicyTemplateProperties()
                {
                    Format = isUrl ? "rawxml-link" : "rawxml",
                    Value = isUrl ? globalServicePolicy : this._fileReader.RetrieveLocalFileContents(globalServicePolicy)
                },
                DependsOn = new string[] { }
            };

            template.resources = new TemplateResource[1]
            {
                policyTemplateResource
            }; 

            return await Task.FromResult(template);
        }

        public PolicyTemplateResource CreateAPIPolicyTemplateResource(ApiConfiguration api, string[] dependsOn)
        {
            Uri uriResult;
            bool isUrl = Uri.TryCreate(api.policy, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            // create policy resource with properties
            PolicyTemplateResource policyTemplateResource = new PolicyTemplateResource(ResourceType.ApiPolicy)
            {
                Name = $"[concat(parameters('ApimServiceName'), '/{api.name}/policy')]",
                Properties = new PolicyTemplateProperties()
                {
                    // if policy is a url inline the url, if it is a local file inline the file contents
                    Format = isUrl ? "rawxml-link" : "rawxml",
                    Value = isUrl ? api.policy : this._fileReader.RetrieveLocalFileContents(api.policy)
                },
                DependsOn = dependsOn
            };
            return policyTemplateResource;
        }

        

        public PolicyTemplateResource CreateOperationPolicyTemplateResource(KeyValuePair<string, OperationsConfig> policyPair, string apiName, string[] dependsOn)
        {
            Uri uriResult;
            bool isUrl = Uri.TryCreate(policyPair.Value.Policy, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            // create policy resource with properties
            PolicyTemplateResource policyTemplateResource = new PolicyTemplateResource(ResourceType.ApiOperationPolicy)
            {
                Name = $"[concat(parameters('ApimServiceName'), '/{apiName}/{policyPair.Key}/policy')]",
                Properties = new PolicyTemplateProperties()
                {
                    // if policy is a url inline the url, if it is a local file inline the file contents
                    Format = isUrl ? "rawxml-link" : "rawxml",
                    Value = isUrl ? policyPair.Value.Policy : this._fileReader.RetrieveLocalFileContents(policyPair.Value.Policy)
                },
                DependsOn = dependsOn
            };
            return policyTemplateResource;
        }

        public List<PolicyTemplateResource> CreateOperationPolicyTemplateResources(ApiConfiguration api, string[] dependsOn)
        {
            // create a policy resource for each policy listed in the config file and its associated provided xml file
            List<PolicyTemplateResource> policyTemplateResources = new List<PolicyTemplateResource>();
            foreach (KeyValuePair<string, OperationsConfig> pair in api.operations)
            {
                policyTemplateResources.Add(this.CreateOperationPolicyTemplateResource(pair, api.name, dependsOn));
            }
            return policyTemplateResources;
        }

    }
}
