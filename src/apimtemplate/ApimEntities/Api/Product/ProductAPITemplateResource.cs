
namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class ProductApiTemplateResource : TemplateResource<ProductApiTemplateProperties>
    {
        public override string Type => ResourceType.ProductApi;
    }

    public class ProductApiTemplateProperties { }
}