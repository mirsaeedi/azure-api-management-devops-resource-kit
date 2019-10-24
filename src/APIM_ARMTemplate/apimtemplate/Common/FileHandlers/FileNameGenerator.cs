using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class FileNameGenerator
    {
        public FileNames GenerateFileNames()
        {
            return new FileNames()
            {
                apiVersionSets = $@"apiVersionSets.template.json",
                authorizationServers = $@"authorizationServers.template.json",
                backends = $@"backends.template.json",
                globalServicePolicy = $@"globalServicePolicy.template.json",
                loggers = $@"loggers.template.json",
                namedValues = $@"namedValues.template.json",
                products = $@"products.template.json",
                parameters = $@"parameters.json",
                linkedMaster = $@"master.template.json"
            };
        }

        public string GenerateCreatorAPIFileName(string apiName, bool isSplitAPI, bool isInitialAPI, string apimServiceName)
        {
            // in case the api name has been appended with ;rev={revisionNumber}, take only the api name initially provided by the user in the creator config
            string sanitizedAPIName = GenerateOriginalAPIName(apiName);
            if (isSplitAPI == true)
            {
                return isInitialAPI == true ? $@"{apimServiceName}-{sanitizedAPIName}-initial.api.template.json" : $@"{apimServiceName}-{sanitizedAPIName}-subsequent.api.template.json";
            }
            else
            {
                return $@"{apimServiceName}-{sanitizedAPIName}.api.template.json";
            }
        }

        public string GenerateExtractorAPIFileName(string singleAPIName, string apimServiceName)
        {
            return singleAPIName == null ? $@"/{apimServiceName}-apis.template.json" : $@"/{apimServiceName}-{singleAPIName}-api.template.json";
        }

        public string GenerateOriginalAPIName(string apiName)
        {
            // in case the api name has been appended with ;rev={revisionNumber}, take only the api name initially provided by the user in the creator config
            string originalName = apiName.Split(";")[0];
            return originalName;
        }
    }

    public class FileNames
    {
        public string apiVersionSets { get; set; }
        public string authorizationServers { get; set; }
        public string backends { get; set; }
        public string globalServicePolicy { get; set; }
        public string loggers { get; set; }
        public string namedValues { get; set; }
        public string products { get; set; }
        public string parameters { get; set; }
        // linked property outputs 1 master template
        public string linkedMaster { get; set; }
    }
}
