
namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{

    public class PolicyTemplateResource : TemplateResource<PolicyTemplateProperties>
    {
        private string _type;
        public PolicyTemplateResource(string resourceType)
        {
            _type = resourceType;
        }
        public override string Type => _type;
    }

    public class PolicyTemplateProperties
    {
        public string Value { get; set; }
        public string Format { get; set; }
    }
}
