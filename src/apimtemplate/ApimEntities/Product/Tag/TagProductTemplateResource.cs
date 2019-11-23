
namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class TagProductTemplateResource : TemplateResource<TagApiTemplateProperties>
    {
        public override string Type => ResourceType.TagProduct;
    }

    public class TagProductTemplateProperties { }
}