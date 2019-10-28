using Xunit;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Test
{
    public class ProductTemplateCreatorTests
    {
        [Fact]
        public void ShouldCreateProductFromCreatorConfig()
        {
            // arrange
            ProductTemplateCreator productTemplateCreator = ProductTemplateCreatorFactory.GenerateProductTemplateCreator();
            CreatorConfig creatorConfig = new CreatorConfig() { Products = new List<ProductConfig>() };
            ProductConfig product = new ProductConfig()
            {
                DisplayName = "displayName",
                Description = "description",
                Terms = "terms",
                SubscriptionRequired = true,
                ApprovalRequired = true,
                SubscriptionsLimit = 1,
                State = "state"
            };
            creatorConfig.Products.Add(product);

            // act
            Template productTemplate = productTemplateCreator.CreateProductTemplate(creatorConfig);
            ProductsTemplateResource productsTemplateResource = (ProductsTemplateResource)productTemplate.Resources[0];

            // assert
            Assert.Equal($"[concat(parameters('ApimServiceName'), '/{product.DisplayName}')]", productsTemplateResource.Name);
            Assert.Equal(product.DisplayName, productsTemplateResource.properties.displayName);
            Assert.Equal(product.Description, productsTemplateResource.properties.description);
            Assert.Equal(product.Terms, productsTemplateResource.properties.terms);
            Assert.Equal(product.SubscriptionsLimit, productsTemplateResource.properties.subscriptionsLimit);
            Assert.Equal(product.SubscriptionRequired, productsTemplateResource.properties.subscriptionRequired);
            Assert.Equal(product.ApprovalRequired, productsTemplateResource.properties.approvalRequired);
            Assert.Equal(product.State, productsTemplateResource.properties.state);
        }

        [Fact]
        public void ShouldNotCreateApprovalRequiredOrSubscriptionsLimitIfSubscriptionRequiredIsFalse()
        {
            // arrange
            ProductTemplateCreator productTemplateCreator = ProductTemplateCreatorFactory.GenerateProductTemplateCreator();
            CreatorConfig creatorConfig = new CreatorConfig() { Products = new List<ProductConfig>() };
            ProductConfig product = new ProductConfig()
            {
                DisplayName = "displayName",
                Description = "description",
                Terms = "terms",
                SubscriptionRequired = false,
                ApprovalRequired = true,
                SubscriptionsLimit = 1,
                State = "state"
            };
            creatorConfig.Products.Add(product);

            // act
            Template productTemplate = productTemplateCreator.CreateProductTemplate(creatorConfig);
            ProductsTemplateResource productsTemplateResource = (ProductsTemplateResource)productTemplate.Resources[0];

            // assert
            Assert.Equal($"[concat(parameters('ApimServiceName'), '/{product.DisplayName}')]", productsTemplateResource.Name);
            Assert.Equal(product.DisplayName, productsTemplateResource.properties.displayName);
            Assert.Equal(product.Description, productsTemplateResource.properties.description);
            Assert.Equal(product.Terms, productsTemplateResource.properties.terms);
            Assert.Equal(product.SubscriptionRequired, productsTemplateResource.properties.subscriptionRequired);
            Assert.Null(productsTemplateResource.properties.subscriptionsLimit);
            Assert.Null(productsTemplateResource.properties.approvalRequired);
            Assert.Equal(product.State, productsTemplateResource.properties.state);
        }
    }
}
