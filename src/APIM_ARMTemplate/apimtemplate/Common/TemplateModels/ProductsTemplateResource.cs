using System.Collections.Generic;
namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class ProductsTemplateResource : TemplateResource<ProductsTemplateProperties>
    {
        public override string Type => ResourceType.Product;
    }

    public class ProductsTemplateProperties
    {
        public string Description { get; set; }
        public string Terms { get; set; }
        public bool SubscriptionRequired { get; set; }
        public bool? ApprovalRequired { get; set; }
        public int? SubscriptionsLimit { get; set; }
        public string State { get; set; }
        public string DisplayName { get; set; }
    }
}