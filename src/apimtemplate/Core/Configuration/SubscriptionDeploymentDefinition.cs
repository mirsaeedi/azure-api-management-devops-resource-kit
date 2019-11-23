using Apim.DevOps.Toolkit.ApimEntities.Tag;
using Apim.DevOps.Toolkit.ArmTemplates;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
	public class SubscriptionDeploymentDefinition : SubscriptionProperties
	{
		/// <summary>
		/// The Id of the Subscription
		/// </summary>
		public string Name { get; set; }
	}

}