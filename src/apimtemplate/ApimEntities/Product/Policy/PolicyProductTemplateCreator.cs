using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Apim.DevOps.Toolkit.Extensions;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class PolicyProductTemplateCreator
    {
        public async Task<PolicyProductTemplateResource> Create(ProductDeploymentDefinition product, string[] dependsOn)
        {
			var fileReader = new FileReader();

			var isUrl = product.Policy.IsUri(out _);

			var policyTemplateResource = new PolicyProductTemplateResource()
			{
				Name = $"[concat(parameters('ApimServiceName'), '/{product.Name}/policy')]",
				Properties = new PolicyProductProperties()
				{
					Format = isUrl ? "rawxml-link" : "rawxml",
					Value = isUrl ? product.Policy : await fileReader.RetrieveFileContentsAsync(product.Policy)
				},
				DependsOn = dependsOn
			};

			return policyTemplateResource;
		}
	}
}
