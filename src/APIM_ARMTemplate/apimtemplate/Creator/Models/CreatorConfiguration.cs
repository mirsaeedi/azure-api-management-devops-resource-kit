using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class CliCreatorArguments
    {
        public string configFile { get; set; }
    }

    public class CreatorConfig
    {
        public string Version { get; set; }
        public string ApimServiceName { get; set; }
        // policy file location (local or url)
        public string Policy { get; set; }
        public List<APIVersionSetConfig> ApiVersionSets { get; set; }
        public List<ApiConfiguration> Apis { get; set; }
        public List<ProductConfig> Products { get; set; }
        public List<LoggerConfig> Loggers { get; set; }
        public List<AuthorizationServerTemplateProperties> AuthorizationServers { get; set; }
        public List<BackendTemplateProperties> Backends { get; set; }
        public string OutputLocation { get; set; }
        public bool Linked { get; set; }
        public string LinkedTemplatesBaseUrl { get; set; }
        public string LinkedTemplatesUrlQueryString { get; set; }
    }

    public class APIVersionSetConfig: ApiVersionSetProperties
    {
        public string id { get; set; }
    }

    public class ApiConfiguration
    {
        // used to build displayName and resource name from APITemplateResource schema
        public string name { get; set; }
        public string description { get; set; }
        public string serviceUrl { get; set; }
        // used to build type and apiType from APITemplateResource schema
        public string type { get; set; }
        // openApiSpec file location (local or url), used to build protocols, value, and format from APITemplateResource schema
        public string openApiSpec { get; set; }
        // policy file location (local or url)
        public string policy { get; set; }
        // used to buld path from APITemplateResource schema
        public string suffix { get; set; }
        public bool subscriptionRequired { get; set; }
        public bool isCurrent { get; set; }
        public string apiVersion { get; set; }
        public string apiVersionDescription { get; set; }
        public string apiVersionSetId { get; set; }
        public string apiRevision { get; set; }
        public string apiRevisionDescription { get; set; }
        public Dictionary<string, OperationsConfig> operations { get; set; }
        public APITemplateAuthenticationSettings authenticationSettings { get; set; }
        public string products { get; set; }
        public string protocols { get; set; }
        public DiagnosticConfig diagnostic { get; set; }
        // does not currently include subscriptionKeyParameterNames, sourceApiId, and wsdlSelector from APITemplateResource schema

        public async Task<bool> IsDependOnLogger(FileReader fileReader)
        {
            if (diagnostic != null && diagnostic.LoggerId != null)
            {
                return true;
            }

            string apiPolicy = policy != null ? await fileReader.RetrieveFileContentsAsync(policy) : "";
            
            if (apiPolicy.Contains("logger"))
            {
                return true;
            }
            
            if (operations != null)
            {
                foreach (KeyValuePair<string, OperationsConfig> operation in operations)
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

        public async Task<bool> IsDependOnBackend(FileReader fileReader)
        {
            string apiPolicy = policy != null ? await fileReader.RetrieveFileContentsAsync(policy) : "";

            if (apiPolicy.Contains("set-backend-service"))
            {
                return true;
            }

            if (operations != null)
            {
                foreach (KeyValuePair<string, OperationsConfig> operation in operations)
                {
                    string operationPolicy = operation.Value.Policy != null ? await fileReader.RetrieveFileContentsAsync(operation.Value.Policy) : "";
                    if (operationPolicy.Contains("set-backend-service"))
                    {
                        // capture operation policy dependent on backend
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsSplitApi()
        {
            // the api needs to be split into multiple templates if the user has supplied a version or version set - deploying swagger related properties at the same time as api version related properties fails, so they must be written and deployed separately
            return apiVersion != null || apiVersionSetId != null || (authenticationSettings != null && authenticationSettings.OAuth2 != null && authenticationSettings.OAuth2.AuthorizationServerId != null);
        }
    }

    public class OperationsConfig
    {
        // policy file location (local or url)
        public string Policy { get; set; }
    }

    public class DiagnosticConfig : DiagnosticTemplateProperties
    {
        public string Name { get; set; }
    }

    public class LoggerConfig : LoggerTemplateProperties
    {
        public string Name { get; set; }
    }

    public class ProductConfig : ProductsTemplateProperties
    {
        // policy file location (local or url)
        public string Policy { get; set; }
    }
    
}
