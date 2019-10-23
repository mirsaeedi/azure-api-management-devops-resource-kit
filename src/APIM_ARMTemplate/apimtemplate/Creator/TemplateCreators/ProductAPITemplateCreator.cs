using System.Collections.Generic;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class ProductAPITemplateCreator
    {
        private ProductAPITemplateResource CreateProductAPITemplateResource(string productID, string apiName, string[] dependsOn)
        {
            // create products/apis resource with properties
            ProductAPITemplateResource productAPITemplateResource = new ProductAPITemplateResource()
            {
                Name = $"[concat(parameters('ApimServiceName'), '/{productID}/{apiName}')]",
                Properties = new ProductAPITemplateProperties(),
                DependsOn = dependsOn
            };
            return productAPITemplateResource;
        }

        public List<ProductAPITemplateResource> CreateProductAPITemplateResources(ApiConfiguration api, string[] dependsOn)
        {
            // create a products/apis association resource for each product provided in the config file
            List<ProductAPITemplateResource> productAPITemplates = new List<ProductAPITemplateResource>();
            // products is comma separated list of productIds
            string[] productIDs = api.products.Split(", ");
            foreach (string productID in productIDs)
            {
                ProductAPITemplateResource productAPITemplate = this.CreateProductAPITemplateResource(productID, api.name, dependsOn);
                productAPITemplates.Add(productAPITemplate);
            }
            return productAPITemplates;
        }
    }
}
