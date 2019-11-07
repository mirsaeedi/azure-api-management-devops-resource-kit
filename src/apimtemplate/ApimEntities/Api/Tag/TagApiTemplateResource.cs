
namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class TagApiTemplateResource : TemplateResource<TagApiTemplateProperties>
    {
        public override string Type => ResourceType.TagApi;
    }

    public class TagApiTemplateProperties { }
}