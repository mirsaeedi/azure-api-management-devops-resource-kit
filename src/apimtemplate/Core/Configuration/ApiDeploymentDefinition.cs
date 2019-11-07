using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apim.DevOps.Toolkit.ArmTemplates;
using Apim.DevOps.Toolkit.Extensions;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
	public class ApiDeploymentDefinition:ApiProperties
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
		public new string Protocols { get; set; }

		public DiagnosticDeploymentDefinition Diagnostic { get; set; }
		public async Task<bool> IsDependOnLogger(FileReader fileReader)
		{
			if (Diagnostic != null && Diagnostic.LoggerId != null)
			{
				return true;
			}

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
		public bool IsDependOnProducts() => Products != null;

		public bool IsDependOnTags() => Tags != null;
		
		public bool IsDependOnApiVersionSet() => ApiVersionSetId != null;
		
		public bool IsDependOnGlobalServicePolicies(DeploymentDefinition creatorConfig) => creatorConfig.Policy != null;

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

		public IEnumerable<string> TagList => Tags.GetItems(new string[0]);
	}

}