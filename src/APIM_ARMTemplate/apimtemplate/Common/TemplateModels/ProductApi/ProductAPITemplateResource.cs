
namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class ProductAPITemplateResource : TemplateResource<ProductAPITemplateProperties>
    {
        public override string Type => ResourceType.ProductAPI;
    }

    public class ProductAPITemplateProperties { }
}