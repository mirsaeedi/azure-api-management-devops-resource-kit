using Apim.DevOps.Toolkit.ApimEntities.Tag;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
	public class TagDeploymentDefinition : TagPropertise
	{
		/// <summary>
		/// The Id of the tag
		/// </summary>
		public string Name { get; set; }
	}

}