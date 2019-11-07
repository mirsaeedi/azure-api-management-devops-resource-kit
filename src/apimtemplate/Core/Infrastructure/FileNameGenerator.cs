using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using System;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class FileNameGenerator
    {
		private string _masterTemplateName;
		private string _prefix;
        public FileNameGenerator(string prefix, string masterTemplateName)
        {
			_masterTemplateName = string.IsNullOrEmpty(masterTemplateName) ? "master.template" : masterTemplateName;
			_prefix = string.IsNullOrEmpty(prefix) ? "" : prefix;
        }

        public FileNames GenerateFileNames()
        {
            return new FileNames()
            {
                ApiVersionSets = $@"{_prefix}apiVersionSets.template.json",
                AuthorizationServers = $@"{_prefix}authorizationServers.template.json",
                Backends = $@"{_prefix}backends.template.json",
                GlobalServicePolicy = $@"{_prefix}globalServicePolicy.template.json",
                Loggers = $@"{_prefix}loggers.template.json",
                NamedValues = $@"{_prefix}namedValues.template.json",
				Products = $@"{_prefix}products.template.json",
				Tags = $@"{_prefix}tags.template.json",
                Parameters = $@"{_prefix}parameters.json",
                LinkedMaster = $@"{_prefix}{_masterTemplateName}.json"
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
        public string ApiVersionSets { get; set; }
        public string AuthorizationServers { get; set; }
        public string Backends { get; set; }
        public string GlobalServicePolicy { get; set; }
        public string Loggers { get; set; }
        public string NamedValues { get; set; }
        public string Products { get; set; }
		public string Tags { get; set; }
		public string Parameters { get; set; }
        // linked property outputs 1 master template
        public string LinkedMaster { get; set; }
    }
}
