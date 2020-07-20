using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apim.DevOps.Toolkit.ApimEntities.Api;
using Apim.DevOps.Toolkit.Core.Infrastructure;
using Apim.DevOps.Toolkit.Extensions;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.ApimEntities
{
	public class ApiDeploymentDefinition
	{
		/// <summary>
		/// The Id of the Api
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// local path or url to policy
		/// </summary>
		public string OpenApiSpec { get; set; }

		/// <summary>
		/// local path or url to policy
		/// </summary>
		public string Policy { get; set; }

		public Dictionary<string, OperationsDeploymentDefinition> Operations { get; set; }

		public string Products { get; set; }

		public string Tags { get; set; }

		public string Protocols { get; set; }

		public string Description { get; set; }

		public ApiTemplateAuthenticationSettings AuthenticationSettings { get; set; }

		public ApiTemplateSubscriptionKeyParameterNames SubscriptionKeyParameterNames { get; set; }

		public string Type { get; set; }

		public string ApiRevision { get; set; }

		public string ApiVersion { get; set; }

		public bool? IsCurrent { get; set; }

		public string ApiRevisionDescription { get; set; }

		public string ApiVersionDescription { get; set; }

		public string ApiVersionSetId { get; set; }

		public bool? SubscriptionRequired { get; set; }

		public string SourceApiId { get; set; }

		public string DisplayName { get; set; }

		public string ServiceUrl { get; set; }

		public string Path { get; set; }

		public ApiTemplateWSDLSelector WsdlSelector { get; set; }

		public string ApiType { get; set; }

		public async Task<bool> IsDependOnLogger(FileReader fileReader)
		{
			/*if (Diagnostic != null && Diagnostic.LoggerId != null)
			{
				return true;
			}*/

			string apiPolicy = Policy != null ? await fileReader.RetrieveFileContentsAsync(Policy) : "";

			if (apiPolicy.Contains("logger"))
			{
				return true;
			}

			if (Operations != null)
			{
				foreach (KeyValuePair<string, OperationsDeploymentDefinition> operation in Operations)
				{
					string operationPolicy = operation.Value.Policy != null ? await fileReader.RetrieveFileContentsAsync(operation.Value.Policy) : "";
					if (operationPolicy.Contains("logger"))
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool IsDependentOnProducts() => Products != null;

		public bool IsDependentOnTags() => Tags != null;

		public bool IsDependOnApiVersionSet() => ApiVersionSetId != null;

		public bool IsDependOnGlobalServicePolicies(DeploymentDefinition deploymentDefinition) => deploymentDefinition.Policy != null;

		public bool IsDependOnAuthorizationServers() => AuthenticationSettings != null &&
			AuthenticationSettings.OAuth2 != null &&
			AuthenticationSettings.OAuth2.AuthorizationServerId != null;

		public async Task<bool> IsDependOnBackend(FileReader fileReader)
		{
			string apiPolicy = Policy != null ? await fileReader.RetrieveFileContentsAsync(Policy) : "";

			if (apiPolicy.Contains("set-backend-service"))
			{
				return true;
			}

			if (Operations == null)
			{
				return false;
			}

			foreach (KeyValuePair<string, OperationsDeploymentDefinition> operation in Operations)
			{
				string operationPolicy = operation.Value.Policy != null ? await fileReader.RetrieveFileContentsAsync(operation.Value.Policy) : "";
				if (operationPolicy.Contains("set-backend-service"))
				{
					return true;
				}
			}

			return false;
		}

		public IEnumerable<string> ProductList => Products.GetItems(new string[0]);

		/// <summary>
		/// Contains Name or DisplayName of tags
		/// </summary>
		public IEnumerable<string> TagList => Tags.GetItems(new string[0]);

		public DeploymentDefinition Root { get; set; }

		internal object GetProductName(string productDisplayName)
		{
			return Root.Products.FirstOrDefault(q => q.DisplayName == productDisplayName || q.Name == productDisplayName)?.Name ?? productDisplayName;
		}

		internal object GetTagName(string tagDisplayName)
		{
			return Root.Tags.FirstOrDefault(q => q.DisplayName == tagDisplayName || q.Name == tagDisplayName)?.Name ?? tagDisplayName;
		}

		internal bool HasPolicy()
		{
			return Policy != null;
		}
	}
}