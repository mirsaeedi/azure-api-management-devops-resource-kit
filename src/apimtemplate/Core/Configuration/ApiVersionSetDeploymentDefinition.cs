using Apim.DevOps.Toolkit.ArmTemplates;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
	public class ApiVersionSetDeploymentDefinition : ApiVersionSetProperties
	{
		/// <summary>
		/// The Id of the Api Version Set
		/// </summary>
		public string Name { get; set; }
	}

}