using System;
using System.Threading.Tasks;
using Apim.DevOps.Toolkit.Extensions;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class ApiPolicyTemplateCreator
    {
        public async Task<ApiPolicyTemplateResource> Create(ApiDeploymentDefinition api, string[] dependsOn)
        {
			var fileReader = new FileReader();

			var isUrl = api.Policy.IsUri(out _);

			var policyTemplateResource = new ApiPolicyTemplateResource()
			{
				Name = $"[concat(parameters('ApimServiceName'), '/{api.Name}/policy')]",
				Properties = new ApiPolicyProperties()
				{
					Format = isUrl ? "rawxml-link" : "rawxml",
					Value = isUrl ? api.Policy : await fileReader.RetrieveFileContentsAsync(api.Policy)
				},
				DependsOn = dependsOn
			};

			return policyTemplateResource;
		}
	}
}
