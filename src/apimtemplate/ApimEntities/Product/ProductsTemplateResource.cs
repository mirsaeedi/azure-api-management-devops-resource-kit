using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class ProductsTemplateResource : TemplateResource<ProductsProperties>
    {
        public override string Type => ResourceType.Product;
    }
}