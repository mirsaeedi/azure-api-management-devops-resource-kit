using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using System;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class FileNameGenerator
    {
        private string _prefix;
        public FileNameGenerator(string prefix)
        {
            _prefix = string.IsNullOrEmpty(prefix) ? "" : prefix.Replace("$datetime", DateTime.Now.ToString("yyyyMMdd-HHmm")) + "-";
        }

        public FileNames GenerateFileNames()
        {
            return new FileNames()
            {
                apiVersionSets = $@"{_prefix}apiVersionSets.template.json",
                authorizationServers = $@"{_prefix}authorizationServers.template.json",
                backends = $@"{_prefix}backends.template.json",
                globalServicePolicy = $@"{_prefix}globalServicePolicy.template.json",
                loggers = $@"{_prefix}loggers.template.json",
                namedValues = $@"{_prefix}namedValues.template.json",
                products = $@"{_prefix}products.template.json",
                parameters = $@"{_prefix}parameters.json",
                linkedMaster = $@"{_prefix}master.template.json"
            };
        }

        public string GenerateCreatorAPIFileName(string apiName, bool isSplitAPI, bool isInitialAPI, string apimServiceName)
        {
            // in case the api name has been appended with ;rev={revisionNumber}, take only the api name initially provided by the user in the creator config
            string sanitizedAPIName = GenerateOriginalAPIName(apiName);
            if (isSplitAPI == true)
            {
                return isInitialAPI == true ? $@"{_prefix}{sanitizedAPIName}-initial.api.template.json" : $@"{_prefix}{sanitizedAPIName}-subsequent.api.template.json";
            }
            else
            {
                return $@"{_prefix}{apimServiceName}-{sanitizedAPIName}.api.template.json";
            }
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
