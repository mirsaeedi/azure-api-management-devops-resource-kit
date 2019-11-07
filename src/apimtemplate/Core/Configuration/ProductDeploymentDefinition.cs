using Apim.DevOps.Toolkit.ArmTemplates;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
	public class ProductDeploymentDefinition : ProductsProperties
	{
		/// <summary>
		/// The Id of the product
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Local path or url to policy
		/// </summary>
		public string Policy { get; set; }
	}

}