using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
	public class ConfigurationValidator
	{
		public ConfigurationValidator()
		{
		}

		public bool Validate(CreatorConfig creatorConfig)
		{
			// ensure required parameters have been passed in

			return ValidateBaseProperties(creatorConfig)
				&& ValidateAPIs(creatorConfig)
				&& ValidateAPIVersionSets(creatorConfig)
				&& ValidateProducts(creatorConfig)
				&& ValidateLoggers(creatorConfig)
				&& ValidateAuthorizationServers(creatorConfig)
				&& ValidateBackends(creatorConfig);
		}

		private bool ValidateProducts(CreatorConfig creatorConfig)
		{
			if (creatorConfig.Products != null)
			{
				foreach (var product in creatorConfig.Products)
				{
					if (product.DisplayName == null)
					{
						throw new ArgumentException("Display name is required if an Product is provided");
					}
				}
			}

			return true;
		}

		private bool ValidateLoggers(CreatorConfig creatorConfig)
		{
			
			if (creatorConfig.Loggers != null)
			{
				foreach (LoggerConfig logger in creatorConfig.Loggers)
				{
					if (logger.Name == null)
					{

						throw new ArgumentException("Name is required if an Logger is provided");
					}
				}
			}
			return true;
		}

		private bool ValidateBackends(CreatorConfig creatorConfig)
		{
			
			if (creatorConfig.Backends != null)
			{
				foreach (var backend in creatorConfig.Backends)
				{
					if (backend.Title == null)
					{

						throw new ArgumentException("Title is required if a Backend is provided");
					}
				}
			}
			return true;
		}

		private bool ValidateAuthorizationServers(CreatorConfig creatorConfig)
		{
			
			if (creatorConfig.AuthorizationServers != null)
			{
				foreach (var authorizationServer in creatorConfig.AuthorizationServers)
				{
					if (authorizationServer.displayName == null)
					{

						throw new ArgumentException("Display name is required if an Authorization Server is provided");
					}
				}
			}
			return true;
		}

		private bool ValidateBaseProperties(CreatorConfig creatorConfig)
		{
			
			if (creatorConfig.OutputLocation == null)
			{
				
				throw new ArgumentException("Output location is required");
			}
			if (creatorConfig.Version == null)
			{
				
				throw new ArgumentException("Version is required");
			}
			if (creatorConfig.ApimServiceName == null)
			{
				
				throw new ArgumentException("APIM service name is required");
			}
			if (creatorConfig.Linked == true && creatorConfig.LinkedTemplatesBaseUrl == null)
			{
				
				throw new ArgumentException("LinkTemplatesBaseUrl is required for linked templates");
			}
			return true;
		}

		private bool ValidateAPIs(CreatorConfig creatorConfig)
		{
			
			if (creatorConfig.Apis == null)
			{
				throw new ArgumentException("API configuration is required");
			}
			foreach (ApiConfiguration api in creatorConfig.Apis)
			{
				if (api.name == null)
				{
					
					throw new ArgumentException("API name is required");
				}
				if (api.openApiSpec == null)
				{
					
					throw new ArgumentException("Open API Spec is required");
				}
				if (api.suffix == null)
				{
					
					throw new ArgumentException("API suffix is required");
				}
				if (api.operations != null)
				{
					foreach (KeyValuePair<string, OperationsConfig> operation in api.operations)
					{
						if (operation.Value == null || operation.Value.Policy == null)
						{
	
							throw new ArgumentException("Policy XML is required if an API operation is provided");
						}
					}
				}
				if (api.diagnostic != null && api.diagnostic.LoggerId == null)
				{
					
					throw new ArgumentException("LoggerId is required if an API diagnostic is provided");
				}
			}
			return true;
		}

		private bool ValidateAPIVersionSets(CreatorConfig creatorConfig)
		{

			if (creatorConfig.ApiVersionSets != null)
			{
				foreach (APIVersionSetConfig apiVersionSet in creatorConfig.ApiVersionSets)
				{
					if (apiVersionSet != null && apiVersionSet.DisplayName == null)
					{

						throw new ArgumentException("Display name is required if an API Version Set is provided");
					}
				}
			}
			return true;
		}
	}
}
