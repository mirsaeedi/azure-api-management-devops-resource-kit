using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.DevOps.Toolkit.Extensions;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class ApiOperationPolicyTemplateCreator
    {
        public async Task<IEnumerable<ApiOperationPolicyTemplateResource>> Create(ApiDeploymentDefinition api, string[] dependsOn)
        {
			var fileReader = new FileReader();
			var apiOperationPolicyTemplateResources = new List<ApiOperationPolicyTemplateResource>();

			foreach (var pair in api.Operations)
			{
				var operationPolicy = pair.Value.Policy;
				var operationName = pair.Key;

				var isUrl = operationPolicy.IsUri(out _);

				var apiOperationPolicyTemplateResource = new ApiOperationPolicyTemplateResource()
				{
					Name = $"[concat(parameters('ApimServiceName'), '/{api.Name}/{operationName}/policy')]",
					Properties = new ApiOperationPolicyProperties()
					{
						Format = isUrl ? "rawxml-link" : "rawxml",
						Value = isUrl ? api.Policy : await fileReader.RetrieveFileContentsAsync(api.Policy)
					},
					DependsOn = dependsOn
				};

				apiOperationPolicyTemplateResources.Add(apiOperationPolicyTemplateResource);
			}

			return apiOperationPolicyTemplateResources;
		}
	}
}
