namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class ReleaseTemplateResource : TemplateResource<ReleaseTemplateProperties>
    {
        public override string Type => ResourceType.ApiRelease;
    }

    public class ReleaseTemplateProperties
    {
        public string ApiId { get; set; }
        public string Notes { get; set; }
    }
}