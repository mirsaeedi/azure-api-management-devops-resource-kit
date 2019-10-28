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
        public ProductTemplateCreator()
        {
        }

        public async Task<Template> Create(CreatorConfig creatorConfig)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key,ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();

            foreach (ProductConfig product in creatorConfig.Products)
            {
                // create product resource with properties
                var productsTemplateResource = new ProductsTemplateResource()
                {
                    Name = $"[concat(parameters('ApimServiceName'), '/{product.DisplayName}')]",
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
                    DependsOn = new string[] { }
                };

                resources.Add(productsTemplateResource);

                // create product policy resource that depends on the product, if provided
                if (product.Policy != null)
                {
                    string[] dependsOn = new string[] { $"[resourceId('{ResourceType.Product}', parameters('ApimServiceName'), '{product.DisplayName}')]" };
                    var productPolicy = await CreateProductPolicyTemplateResource(product, dependsOn);
                    resources.Add(productPolicy);
                }
            }

            template.Resources = resources.ToArray();
            return await Task.FromResult(template);
        }

        public async Task<PolicyTemplateResource> CreateProductPolicyTemplateResource(ProductConfig product, string[] dependsOn)
        {
            var fileReader = new FileReader();

            bool isUrl = Uri.TryCreate(product.Policy, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            
            var policyTemplateResource = new PolicyTemplateResource(ResourceType.ProductPolicy)
            {
                Name = $"[concat(parameters('ApimServiceName'), '/{product.DisplayName}/policy')]",
                Properties = new PolicyProperties()
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
