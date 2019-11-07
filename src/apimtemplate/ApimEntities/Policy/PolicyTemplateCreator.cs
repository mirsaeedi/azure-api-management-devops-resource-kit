using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Apim.DevOps.Toolkit.ArmTemplates;
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

        public async Task<Template> Create(DeploymentDefinition creatorConfig)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            template.Resources = new TemplateResource[1]
            {
                await CreateOperationPolicyTemplateResource(ResourceType.GlobalServicePolicy, creatorConfig.Policy, $"policy", new string[0])
            }; 

            return await Task.FromResult(template);
        }

        public Task<PolicyTemplateResource> CreateApiPolicyTemplateResource(ApiDeploymentDefinition api, string[] dependsOn)
        {
            return CreateOperationPolicyTemplateResource(ResourceType.ApiPolicy, api.Policy, $"{api.Name}/policy", dependsOn);  
        }

        public async Task<PolicyTemplateResource> CreateOperationPolicyTemplateResource(string policyType ,string policy, string name, string[] dependsOn)
        {
            bool isUrl = Uri.TryCreate(policy, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            
            PolicyTemplateResource policyTemplateResource = new PolicyTemplateResource(policyType)
            {
                Name = $"[concat(parameters('ApimServiceName'), '/{name}')]",
                Properties = new PolicyProperties()
                {
                    // if policy is a url inline the url, if it is a local file inline the file contents
                    Format = isUrl ? "rawxml-link" : "rawxml",
                    Value = isUrl ? policy : await this._fileReader.RetrieveFileContentsAsync(policy)
                },
                DependsOn = dependsOn
            };
            return policyTemplateResource;
        }

        public async Task<List<PolicyTemplateResource>> CreateOperationPolicyTemplateResources(ApiDeploymentDefinition api, string[] dependsOn)
        {
            // create a policy resource for each policy listed in the config file and its associated provided xml file
            List<PolicyTemplateResource> policyTemplateResources = new List<PolicyTemplateResource>();
            foreach (KeyValuePair<string, OperationsDeploymentDefinition> pair in api.Operations)
            {
                policyTemplateResources.Add(await CreateOperationPolicyTemplateResource(ResourceType.ApiOperationPolicy, pair.Value.Policy, $"{api.Name}/{pair.Key}/policy", dependsOn));
            }
            return policyTemplateResources;
        }

    }
}
