using System.Collections.Generic;
using System.Linq;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class ProductApiTemplateCreator
    {
		private IEnumerable<ProductDeploymentDefinition> _products;

		public ProductApiTemplateCreator(IEnumerable<ProductDeploymentDefinition> products)
		{
			_products = products ?? new ProductDeploymentDefinition[0];
		}

        public List<ProductApiTemplateResource> CreateProductApiTemplateResources(ApiDeploymentDefinition api, string[] dependsOn)
        {
            var productApiTemplates = new List<ProductApiTemplateResource>();

			var productDisplayNames = api.ProductList;

            foreach(string productDisplayName in productDisplayNames)
			{
				var productName = GetProductName(productDisplayName);

				var productApiTemplate = new ProductApiTemplateResource()
				{
					Name = $"[concat(parameters('ApimServiceName'), '/{productName}/{api.Name}')]",
					Properties = new ProductApiTemplateProperties(),
					DependsOn = dependsOn
				};

				productApiTemplates.Add(productApiTemplate);
			}
			return productApiTemplates;
        }

		private string GetProductName(string name)
		{
			var productName = _products.SingleOrDefault(q => q.DisplayName == name)?.Name;

			if (productName == null)
			{
				productName = _products.SingleOrDefault(q => q.Name == name)?.Name;
			}

			if (productName == null)
				return name;

			return productName;
		}
	}
}
