
namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
	public class ApiOperationPolicyTemplateResource : TemplateResource<ApiOperationPolicyProperties>
    {
        public override string Type => ResourceType.ApiOperationPolicy;
    }
}