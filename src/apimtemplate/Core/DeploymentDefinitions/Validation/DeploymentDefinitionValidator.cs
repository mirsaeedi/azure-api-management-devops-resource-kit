using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using System;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Validation
{
	public class DeploymentDefinitionValidator
	{
		public bool Validate(DeploymentDefinition deploymentDefinition)
		{
			// ensure required parameters have been passed in

			return ValidateBaseProperties(deploymentDefinition)
				&& ValidateApis(deploymentDefinition)
				&& ValidateApiVersionSets(deploymentDefinition)
				&& ValidateProducts(deploymentDefinition)
				&& ValidateLoggers(deploymentDefinition)
				&& ValidateAuthorizationServers(deploymentDefinition)
				&& ValidateBackends(deploymentDefinition);
		}

		private bool ValidateProducts(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.Products != null)
			{
				foreach (var product in deploymentDefinition.Products)
				{
					if (product.DisplayName == null)
					{
						throw new ArgumentException("Display name is required if an Product is provided");
					}

					if (product.Name == null)
					{
						throw new ArgumentException("Name is required if an Product is provided");
					}
				}
			}

			return true;
		}

		private bool ValidateLoggers(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.Loggers != null)
			{
				foreach (LoggerDeploymentDefinition logger in deploymentDefinition.Loggers)
				{
					if (logger.Name == null)
					{
						throw new ArgumentException("Name is required if an Logger is provided");
					}
				}
			}
			return true;
		}

		private bool ValidateBackends(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.Backends != null)
			{
				foreach (var backend in deploymentDefinition.Backends)
				{
					if (backend.Title == null)
					{
						throw new ArgumentException("Title is required if a Backend is provided");
					}
				}
			}
			return true;
		}

		private bool ValidateAuthorizationServers(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.AuthorizationServers != null)
			{
				foreach (var authorizationServer in deploymentDefinition.AuthorizationServers)
				{
					if (authorizationServer.DisplayName == null)
					{
						throw new ArgumentException("Display name is required if an Authorization Server is provided");
					}
				}
			}
			return true;
		}

		private bool ValidateBaseProperties(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.Version == null)
			{
				throw new ArgumentException("Version is required");
			}
			if (deploymentDefinition.ApimServiceName == null)
			{
				throw new ArgumentException("APIM service name is required");
			}
			return true;
		}

		private bool ValidateApis(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.Apis == null)
			{
				throw new ArgumentException("API configuration is required");
			}
			foreach (ApiDeploymentDefinition api in deploymentDefinition.Apis)
			{
				if (api.Name == null)
				{
					throw new ArgumentException("API name is required");
				}
				if (api.OpenApiSpec == null && api.GraphQlSpec == null)
				{
					throw new ArgumentException("Open API Spec or Graph QL Endpoint is required");
				}
				if (api.OpenApiSpec != null && api.GraphQlSpec != null)
				{
					throw new ArgumentException("Specifying both Open API Spec and Graph QL Endpoint is not allowed");
				}
				if (api.Path == null)
				{
					throw new ArgumentException("API path is required");
				}
				if (api.Operations != null)
				{
					foreach (KeyValuePair<string, OperationsDeploymentDefinition> operation in api.Operations)
					{
						if (operation.Value == null || operation.Value.Policy == null)
						{
							throw new ArgumentException("Policy XML is required if an API operation is provided");
						}
					}
				}
			}
			return true;
		}

		private bool ValidateApiVersionSets(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.ApiVersionSets != null)
			{
				foreach (ApiVersionSetDeploymentDefinition apiVersionSet in deploymentDefinition.ApiVersionSets)
				{
					if (apiVersionSet != null && apiVersionSet.DisplayName == null)
					{
						throw new ArgumentException("Display name is required if an API Version Set is provided");
					}

					if (apiVersionSet != null && apiVersionSet.Name == null)
					{
						throw new ArgumentException("Name is required if an API Version Set is provided");
					}
				}
			}
			return true;
		}
	}
}
