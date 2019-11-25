
namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
	public class PolicyProductTemplateResource : TemplateResource<PolicyProductProperties>
    {
        public override string Type => ResourceType.ProductPolicy;
    }
}