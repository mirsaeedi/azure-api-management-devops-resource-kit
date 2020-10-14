using Apim.DevOps.Toolkit.ApimEntities.Subscription;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities
{
	public class SubscriptionDeploymentDefinition : EntityDeploymentDefinition
	{
		private static string _productRegexPattern = "/products/(?<productName>.+)";

		private static string _apiRegexPattern = "/apis/(?<apiName>.+)";

		private static string _userRegexPattern = "/users/(?<userName>.+)";

		/// <summary>
		/// The Id of the Subscription
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// User (user id path) for whom subscription is being created in form /users/{userId}
		/// </summary>
		public string OwnerId { get; set; }

		/// <summary>
		/// Scope like /products/{productId} or /apis or /apis/{apiId}.
		/// </summary>
		public string Scope { get; set; }


		/// <summary>
		/// Subscription name.
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// Primary subscription key. If not specified during request key will be generated automatically.
		/// </summary>
		public string PrimaryKey { get; set; }

		/// <summary>
		/// Secondary subscription key. If not specified during request key will be generated automatically.
		/// </summary>
		public string SecondaryKey { get; set; }

		/// <summary>
		/// Initial subscription state. If no value is specified, subscription is created with Submitted state. 
		/// Possible states are * active – the subscription is active, * suspended – the subscription is blocked, and the subscriber cannot call any APIs of the product, 
		/// * submitted – the subscription request has been made by the developer, but has not yet been approved or rejected, 
		/// * rejected – the subscription request has been denied by an administrator, * cancelled – the subscription has been cancelled by the developer 
		/// or administrator, * expired – the subscription reached its expiration date and was deactivated. - suspended, active, expired, 
		/// submitted, rejected, cancelled
		/// </summary>
		public SubscriptionState? State { get; set; }

		/// <summary>
		/// Determines whether tracing can be enabled
		/// </summary>
		public bool? AllowTracing { get; set; }

		public override IEnumerable<string> Dependencies()
		{
			var dependencies = new List<string>();
			var dependentProduct = DependentProduct();
			var dependentApi = DependentApi();
			var dependentUser = DependentUser();

			if (dependentProduct != null)
			{
				dependencies.Add($"[resourceId('{ResourceType.Product}', parameters('ApimServiceName'), '{dependentProduct}')]");
			}

			if (dependentApi != null)
			{
				dependencies.Add($"[resourceId('{ResourceType.Api}', parameters('ApimServiceName'), '{dependentApi}')]");
			}

			if (dependentUser != null)
			{
				dependencies.Add($"[resourceId('{ResourceType.User}', parameters('ApimServiceName'), '{dependentUser}')]");
			}

			return dependencies;
		}

		private string DependentProduct()
		{
			var regex = new Regex(_productRegexPattern, RegexOptions.IgnoreCase);
			var match = regex.Match(Scope);

			if (match.Success)
			{
				return $"{match.Groups["productName"].Value}";
			}

			return null;
		}

		private string DependentApi()
		{
			var regex = new Regex(_apiRegexPattern, RegexOptions.IgnoreCase);
			var match = regex.Match(Scope);

			if (match.Success)
			{
				return $"{match.Groups["apiName"].Value}";
			}

			return null;
		}

		private string DependentUser()
		{
			var regex = new Regex(_userRegexPattern, RegexOptions.IgnoreCase);
			var match = regex.Match(OwnerId);

			if (match.Success)
			{
				return $"{match.Groups["userName"].Value}";
			}

			return null;
		}
	}
}