
namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class APIVersionSetTemplateResource : TemplateResource<ApiVersionSetProperties>
    {
        public override string Type => ResourceType.ApiVersionSet;
    }

    public class ApiVersionSetProperties
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string VersionQueryName { get; set; }
        public string VersionHeaderName { get; set; }
        public string VersioningScheme { get; set; }
    }
}
