
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class CreatorConfigurationValidator
    {
        private CommandLineApplication commandLineApplication;

        public CreatorConfigurationValidator(CommandLineApplication commandLineApplication)
        {
            this.commandLineApplication = commandLineApplication;
        }

        public bool ValidateCreatorConfig(CreatorConfig creatorConfig)
        {
            // ensure required parameters have been passed in
            if (ValidateBaseProperties(creatorConfig) != true)
            {
                return false;
            }
            if (ValidateAPIs(creatorConfig) != true)
            {
                return false;
            }
            if (ValidateAPIVersionSets(creatorConfig) != true)
            {
                return false;
            }
            if (ValidateProducts(creatorConfig) != true)
            {
                return false;
            }
            if (ValidateLoggers(creatorConfig) != true)
            {
                return false;
            }
            if (ValidateAuthorizationServers(creatorConfig) != true)
            {
                return false;
            }
            if (ValidateBackends(creatorConfig) != true)
            {
                return false;
            }
            return true;
        }

        public bool ValidateProducts(CreatorConfig creatorConfig)
        {
            bool isValid = true;
            if (creatorConfig.Products != null)
            {
                foreach (ProductsTemplateProperties product in creatorConfig.Products)
                {
                    if (product.DisplayName == null)
                    {
                        isValid = false;
                        throw new CommandParsingException(commandLineApplication, "Display name is required if an Product is provided");
                    }
                }
            }
            return isValid;
        }

        public bool ValidateLoggers(CreatorConfig creatorConfig)
        {
            bool isValid = true;
            if (creatorConfig.Loggers != null)
            {
                foreach (LoggerConfig logger in creatorConfig.Loggers)
                {
                    if (logger.Name == null)
                    {
                        isValid = false;
                        throw new CommandParsingException(commandLineApplication, "Name is required if an Logger is provided");
                    }
                }
            }
            return isValid;
        }

        public bool ValidateBackends(CreatorConfig creatorConfig)
        {
            bool isValid = true;
            if (creatorConfig.Backends != null)
            {
                foreach (BackendTemplateProperties backend in creatorConfig.Backends)
                {
                    if (backend.Title == null)
                    {
                        isValid = false;
                        throw new CommandParsingException(commandLineApplication, "Title is required if a Backend is provided");
                    }
                }
            }
            return isValid;
        }

        public bool ValidateAuthorizationServers(CreatorConfig creatorConfig)
        {
            bool isValid = true;
            if (creatorConfig.AuthorizationServers != null)
            {
                foreach (AuthorizationServerTemplateProperties authorizationServer in creatorConfig.AuthorizationServers)
                {
                    if (authorizationServer.displayName == null)
                    {
                        isValid = false;
                        throw new CommandParsingException(commandLineApplication, "Display name is required if an Authorization Server is provided");
                    }
                }
            }
            return isValid;
        }

        public bool ValidateBaseProperties(CreatorConfig creatorConfig)
        {
            bool isValid = true;
            if (creatorConfig.OutputLocation == null)
            {
                isValid = false;
                throw new CommandParsingException(commandLineApplication, "Output location is required");
            }
            if (creatorConfig.Version == null)
            {
                isValid = false;
                throw new CommandParsingException(commandLineApplication, "Version is required");
            }
            if (creatorConfig.ApimServiceName == null)
            {
                isValid = false;
                throw new CommandParsingException(commandLineApplication, "APIM service name is required");
            }
            if (creatorConfig.Linked == true && creatorConfig.LinkedTemplatesBaseUrl == null)
            {
                isValid = false;
                throw new CommandParsingException(commandLineApplication, "LinkTemplatesBaseUrl is required for linked templates");
            }
            return isValid;
        }

        public bool ValidateAPIs(CreatorConfig creatorConfig)
        {
            bool isValid = true;
            if (creatorConfig.Apis == null)
            {
                isValid = false;
                throw new CommandParsingException(commandLineApplication, "API configuration is required");
            }
            foreach (ApiConfiguration api in creatorConfig.Apis)
            {
                if (api.name == null)
                {
                    isValid = false;
                    throw new CommandParsingException(commandLineApplication, "API name is required");
                }
                if (api.openApiSpec == null)
                {
                    isValid = false;
                    throw new CommandParsingException(commandLineApplication, "Open API Spec is required");
                }
                if (api.suffix == null)
                {
                    isValid = false;
                    throw new CommandParsingException(commandLineApplication, "API suffix is required");
                }
                if (api.operations != null)
                {
                    foreach (KeyValuePair<string, OperationsConfig> operation in api.operations)
                    {
                        if (operation.Value == null || operation.Value.Policy == null)
                        {
                            isValid = false;
                            throw new CommandParsingException(commandLineApplication, "Policy XML is required if an API operation is provided");
                        }
                    }
                }
                if (api.diagnostic != null && api.diagnostic.LoggerId == null)
                {
                    isValid = false;
                    throw new CommandParsingException(commandLineApplication, "LoggerId is required if an API diagnostic is provided");
                }
            }
            return isValid;
        }

        public bool ValidateAPIVersionSets(CreatorConfig creatorConfig)
        {
            bool isValid = true;
            if(creatorConfig.ApiVersionSets != null)
            {
                foreach (APIVersionSetConfig apiVersionSet in creatorConfig.ApiVersionSets)
                {
                    if (apiVersionSet != null && apiVersionSet.DisplayName == null)
                    {
                        isValid = false;
                        throw new CommandParsingException(commandLineApplication, "Display name is required if an API Version Set is provided");
                    }
                }
            }
            return isValid;
        }
    }
}
