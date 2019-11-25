using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Apim.DevOps.Toolkit.ArmTemplates;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class ProductTemplateCreator : TemplateCreator,ITemplateCreator
    {
		private TagProductTemplateCreator _tagProductTemplateCreator;
		private PolicyProductTemplateCreator _policyProductTemplateCreator;

		public ProductTemplateCreator(IEnumerable<TagDeploymentDefinition> tags)
		{
			_tagProductTemplateCreator = new TagProductTemplateCreator(tags);
			_policyProductTemplateCreator = new PolicyProductTemplateCreator();
		}
		public async Task<Template> Create(DeploymentDefinition creatorConfig)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();

            foreach (var product in creatorConfig.Products)
            {
                // create product resource with properties
                var productsTemplateResource = new ProductsTemplateResource()
                {
                    Name = $"[concat(parameters('ApimServiceName'), '/{product.Name}')]",
                    Properties = new ProductsProperties()
                    {
                        Description = product.Description,
                        Terms = product.Terms,
                        SubscriptionRequired = product.SubscriptionRequired,
                        ApprovalRequired = product.SubscriptionRequired ? product.ApprovalRequired : null,
                        SubscriptionsLimit = product.SubscriptionRequired ? product.SubscriptionsLimit : null,
                        State = product.State,
                        DisplayName = product.DisplayName
                    },
                    DependsOn =  new string[] { }
                };

                resources.Add(productsTemplateResource);

				resources.AddRange(await CreateChildResourceTemplates(product));
			}

            template.Resources = resources.ToArray();
            return await Task.FromResult(template);
        }

		private async Task<IEnumerable<TemplateResource>> CreateChildResourceTemplates(ProductDeploymentDefinition product)
		{
			var resources = new List<TemplateResource>();

			var dependsOn = new string[] { $"[resourceId('{ResourceType.Product}', parameters('ApimServiceName'), '{product.Name}')]" };

			if (product.IsDependOnPolicy())
			{
				resources.Add(await _policyProductTemplateCreator.Create(product, dependsOn));
			}

			if (product.IsDependOnTags())
			{
				resources.AddRange(_tagProductTemplateCreator.Create(product, dependsOn));
			}

			return resources;
		}

	}
}
