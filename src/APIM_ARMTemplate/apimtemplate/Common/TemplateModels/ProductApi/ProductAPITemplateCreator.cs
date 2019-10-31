using System.Collections.Generic;
using System.Linq;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class ProductAPITemplateCreator
    {
		private IEnumerable<ProductConfig> _products;

		public ProductAPITemplateCreator(IEnumerable<ProductConfig> products)
		{
			_products = products;
		}

		private ProductApoTemplateResource CreateProductApiemplateResource(string productId, string apiName, string[] dependsOn)
        {
            // create products/apis resource with properties
            ProductApoTemplateResource productAPITemplateResource = new ProductApoTemplateResource()
            {
                Name = $"[concat(parameters('ApimServiceName'), '/{productId}/{apiName}')]",
                Properties = new ProductAPITemplateProperties(),
                DependsOn = dependsOn
            };
            return productAPITemplateResource;
        }

        public List<ProductApoTemplateResource> CreateProductAPITemplateResources(ApiConfiguration api, string[] dependsOn)
        {
            // create a products/apis association resource for each product provided in the config file
            List<ProductApoTemplateResource> productAPITemplates = new List<ProductApoTemplateResource>();
            // products is comma separated list of productIds
            var productDisplayNames = api.products.Split(",").Select(p=>p.Trim());
            foreach (string productDisplayName in productDisplayNames)
            {
				var product = _products.Single(q => q.DisplayName == productDisplayName);

                ProductApoTemplateResource productAPITemplate = this.CreateProductApiemplateResource(product.Id, api.name, dependsOn);
                productAPITemplates.Add(productAPITemplate);
            }
            return productAPITemplates;
        }
    }
}
