namespace Apim.DevOps.Toolkit.ApimEntities.Product
{
	public class ProductsProperties
	{
		public string Description { get; set; }

		public string Terms { get; set; }

		public bool SubscriptionRequired { get; set; }

		public bool? ApprovalRequired { get; set; }

		public int? SubscriptionsLimit { get; set; }

		public string State { get; set; }

		public string DisplayName { get; set; }
	}
}
