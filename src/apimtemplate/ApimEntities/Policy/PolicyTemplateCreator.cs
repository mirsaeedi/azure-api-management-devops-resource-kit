using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Apim.DevOps.Toolkit.ArmTemplates;
using Apim.DevOps.Toolkit.Extensions;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class PolicyTemplateCreator: TemplateCreator,ITemplateCreator
    {
        public async Task<Template> Create(DeploymentDefinition creatorConfig)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

			var fileReader = new FileReader();
			var policy = creatorConfig.Policy;
			var isUrl = policy.IsUri(out _);

			var policyTemplateResource = new PolicyTemplateResource()
			{
				Name = $"[concat(parameters('ApimServiceName'), '/policy')]",
				Properties = new PolicyProperties()
				{
					Format = isUrl ? "rawxml-link" : "rawxml",
					Value = isUrl ? policy : await fileReader.RetrieveFileContentsAsync(policy)
				},
				DependsOn = new string[0]
			};

			template.Resources = new TemplateResource[] { policyTemplateResource }; 

            return await Task.FromResult(template);
        }
    }
}
