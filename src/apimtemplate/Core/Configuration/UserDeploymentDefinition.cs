using Apim.DevOps.Toolkit.ApimEntities.Tag;
using Apim.DevOps.Toolkit.ArmTemplates;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
	public class UserDeploymentDefinition : UserProperties
	{
		/// <summary>
		/// The Id of the User
		/// </summary>
		public string Name { get; set; }
	}

}