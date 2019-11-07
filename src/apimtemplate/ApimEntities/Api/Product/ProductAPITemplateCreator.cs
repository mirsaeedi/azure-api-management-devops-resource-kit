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
			_products = products;
		}

        public List<ProductApiTemplateResource> CreateProductApiTemplateResources(ApiDeploymentDefinition api, string[] dependsOn)
        {
            var productApiTemplates = new List<ProductApiTemplateResource>();

			var productDisplayNames = api.ProductList;

            foreach(string productDisplayName in productDisplayNames)
            {
				var product = _products.Single(q => q.DisplayName == productDisplayName);

				var productApiTemplate = new ProductApiTemplateResource()
				{
					Name = $"[concat(parameters('ApimServiceName'), '/{product.Name}/{api.Name}')]",
					Properties = new ProductApiTemplateProperties(),
					DependsOn = dependsOn
				}; 

                productApiTemplates.Add(productApiTemplate);
            }
            return productApiTemplates;
        }
    }
}
