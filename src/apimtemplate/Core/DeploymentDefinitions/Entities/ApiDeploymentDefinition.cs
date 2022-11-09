using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apim.DevOps.Toolkit.ApimEntities.Api;
using Apim.DevOps.Toolkit.ApimEntities.Api.Diagnostics;
using Apim.DevOps.Toolkit.Core.Infrastructure;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using Apim.DevOps.Toolkit.Extensions;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities
{
	public class ApiDeploymentDefinition: EntityDeploymentDefinition
	{
		/// <summary>
		/// The Id of the Api
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// local path or url to OpenAPI specification
		/// </summary>
		public string OpenApiSpec { get; set; }

		/// <summary>
		/// local path or url to GraphQL API specification
		/// </summary>
		public string GraphQlSpec { get; set; }

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

		public ApiDiagnosticsProperties Diagnostics { get; set; }

		public LoggerDeploymentDefinition AssociatedLogger => Root.Loggers?.SingleOrDefault(logger => logger.Name == Diagnostics?.LoggerId);

		public string Type { get; set; }

		/// <summary>
		/// Describes the revision of the API. If no value is provided, default revision 1 is created.
		/// </summary>
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

		public bool IsDependentOnLogger() => Diagnostics != null;

		public bool IsDependentOnApiVersionSet() => ApiVersionSetId != null;

		public bool IsDependentOnAuthorizationServers() => AuthenticationSettings != null &&
			AuthenticationSettings.OAuth2 != null &&
			AuthenticationSettings.OAuth2.AuthorizationServerId != null;

		public async Task<bool> IsDependentOnBackend(FileReader fileReader)
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

		public string GetProductName(string productDisplayName)
		{
			return Root.Products.FirstOrDefault(q => q.DisplayName == productDisplayName || q.Name == productDisplayName)?.Name ?? productDisplayName;
		}

		public string GetTagName(string tagDisplayName)
		{
			return Root.Tags.FirstOrDefault(q => q.DisplayName == tagDisplayName || q.Name == tagDisplayName)?.Name ?? tagDisplayName;
		}

		public bool HasPolicy()
		{
			return Policy != null;
		}
		public bool HasDiagnostics()
		{
			return Diagnostics != null;
		}

		public override IEnumerable<string> Dependencies()
		{
			var dependencies = new List<string>();

			if (IsDependentOnProducts())
			{
				var dependentProducts = ProductList
					.Where(product => Root.Products.Any(p => GetProductName(product) == p.Name))
					.Select(product => $"[resourceId('{ResourceType.Product}', parameters('ApimServiceName'), '{GetProductName(product)}')]");

				dependencies.AddRange(dependentProducts);
			}

			if (IsDependentOnTags())
			{
				var dependentTags = TagList
					.Where(tag => Root.Tags.Any(t => GetTagName(tag) == t.Name))
					.Select(tag => $"[resourceId('{ResourceType.Tag}', parameters('ApimServiceName'), '{GetTagName(tag)}')]");

				dependencies.AddRange(dependentTags);
			}

			if (IsDependentOnLogger())
			{
				var dependentLogger = Root.Loggers.Where(logger => Diagnostics.LoggerId == logger.Name)
					.Select(logger => $"[resourceId('{ResourceType.Logger}', parameters('ApimServiceName'), '{logger.Name}')]");

				dependencies.AddRange(dependentLogger);
			}


			if (IsDependentOnApiVersionSet())
			{
				var dependentApiVersionSet = $"[resourceId('{ResourceType.ApiVersionSet}', parameters('ApimServiceName'), '{ApiVersionSetId}')]";

				dependencies.Add(dependentApiVersionSet);
			}

			return dependencies;
		}
	}
}