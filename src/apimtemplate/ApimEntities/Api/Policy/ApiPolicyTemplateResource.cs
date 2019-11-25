
namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
	public class ApiPolicyTemplateResource : TemplateResource<ApiPolicyProperties>
    {
        public override string Type => ResourceType.ApiPolicy;
    }
}