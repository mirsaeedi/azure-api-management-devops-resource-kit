
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class CreatorConfigurationValidator
    {
        private CommandLineApplication _commandLineApplication;

        public CreatorConfigurationValidator(CommandLineApplication commandLineApplication)
        {
            _commandLineApplication = commandLineApplication;
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
            bool isValid = true;
            if (creatorConfig.Products != null)
            {
                foreach (var product in creatorConfig.Products)
                {
                    if (product.DisplayName == null)
                    {
                        isValid = false;
                        throw new CommandParsingException(_commandLineApplication, "Display name is required if an Product is provided");
                    }
                }
            }
            return isValid;
        }

        private bool ValidateLoggers(CreatorConfig creatorConfig)
        {
            bool isValid = true;
            if (creatorConfig.Loggers != null)
            {
                foreach (LoggerConfig logger in creatorConfig.Loggers)
                {
                    if (logger.Name == null)
                    {
                        isValid = false;
                        throw new CommandParsingException(_commandLineApplication, "Name is required if an Logger is provided");
                    }
                }
            }
            return isValid;
        }

        private bool ValidateBackends(CreatorConfig creatorConfig)
        {
            bool isValid = true;
            if (creatorConfig.Backends != null)
            {
                foreach (var backend in creatorConfig.Backends)
                {
                    if (backend.Title == null)
                    {
                        isValid = false;
                        throw new CommandParsingException(_commandLineApplication, "Title is required if a Backend is provided");
                    }
                }
            }
            return isValid;
        }

        private bool ValidateAuthorizationServers(CreatorConfig creatorConfig)
        {
            bool isValid = true;
            if (creatorConfig.AuthorizationServers != null)
            {
                foreach (var authorizationServer in creatorConfig.AuthorizationServers)
                {
                    if (authorizationServer.displayName == null)
                    {
                        isValid = false;
                        throw new CommandParsingException(_commandLineApplication, "Display name is required if an Authorization Server is provided");
                    }
                }
            }
            return isValid;
        }

        private bool ValidateBaseProperties(CreatorConfig creatorConfig)
        {
            bool isValid = true;
            if (creatorConfig.OutputLocation == null)
            {
                isValid = false;
                throw new CommandParsingException(_commandLineApplication, "Output location is required");
            }
            if (creatorConfig.Version == null)
            {
                isValid = false;
                throw new CommandParsingException(_commandLineApplication, "Version is required");
            }
            if (creatorConfig.ApimServiceName == null)
            {
                isValid = false;
                throw new CommandParsingException(_commandLineApplication, "APIM service name is required");
            }
            if (creatorConfig.Linked == true && creatorConfig.LinkedTemplatesBaseUrl == null)
            {
                isValid = false;
                throw new CommandParsingException(_commandLineApplication, "LinkTemplatesBaseUrl is required for linked templates");
            }
            return isValid;
        }

        private bool ValidateAPIs(CreatorConfig creatorConfig)
        {
            bool isValid = true;
            if (creatorConfig.Apis == null)
            {
                throw new CommandParsingException(_commandLineApplication, "API configuration is required");
            }
            foreach (ApiConfiguration api in creatorConfig.Apis)
            {
                if (api.name == null)
                {
                    isValid = false;
                    throw new CommandParsingException(_commandLineApplication, "API name is required");
                }
                if (api.openApiSpec == null)
                {
                    isValid = false;
                    throw new CommandParsingException(_commandLineApplication, "Open API Spec is required");
                }
                if (api.suffix == null)
                {
                    isValid = false;
                    throw new CommandParsingException(_commandLineApplication, "API suffix is required");
                }
                if (api.operations != null)
                {
                    foreach (KeyValuePair<string, OperationsConfig> operation in api.operations)
                    {
                        if (operation.Value == null || operation.Value.Policy == null)
                        {
                            isValid = false;
                            throw new CommandParsingException(_commandLineApplication, "Policy XML is required if an API operation is provided");
                        }
                    }
                }
                if (api.diagnostic != null && api.diagnostic.LoggerId == null)
                {
                    isValid = false;
                    throw new CommandParsingException(_commandLineApplication, "LoggerId is required if an API diagnostic is provided");
                }
            }
            return isValid;
        }

        private bool ValidateAPIVersionSets(CreatorConfig creatorConfig)
        {
            var isValid = true;
            if(creatorConfig.ApiVersionSets != null)
            {
                foreach (APIVersionSetConfig apiVersionSet in creatorConfig.ApiVersionSets)
                {
                    if (apiVersionSet != null && apiVersionSet.DisplayName == null)
                    {
                        isValid = false;
                        throw new CommandParsingException(_commandLineApplication, "Display name is required if an API Version Set is provided");
                    }
                }
            }
            return isValid;
        }
    }
}
