namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
	public class TemplateFileName
    {
		private string _masterTemplateName;
		private string _prefix;

		public TemplateFileName(string prefix, string masterTemplateName)
		{
			_masterTemplateName = string.IsNullOrEmpty(masterTemplateName) ? "master.template" : masterTemplateName;
			_prefix = string.IsNullOrEmpty(prefix) ? "" : prefix;
		}

        public string ApiVersionSets()
		{
			return $@"{_prefix}apiVersionSets.template.json"; ;
		}
		public string AuthorizationServers()
		{
			return $@"{_prefix}authorizationServers.template.json";
		}
		public string Backends()
		{
			return $@"{_prefix}backends.template.json";
		}
		public string GlobalServicePolicy()
		{
			return $@"{_prefix}globalServicePolicy.template.json";
		}
		public string Loggers()
		{
			return $@"{_prefix}loggers.template.json";
		}
		public string NamedValues()
		{
			return $@"{_prefix}namedValues.template.json";
		}
		public string Products()
		{
			return $@"{_prefix}products.template.json";
		}
		public string Tags()
		{
			return $@"{_prefix}tags.template.json";
		}
		public string Parameters()
		{
			return $@"{_prefix}parameters.json";
		}
        // linked property outputs 1 master template
        public string LinkedMaster()
		{
			return $@"{_prefix}{_masterTemplateName}.json";
		}
		public string Users()
		{
			return $@"{_prefix}users.template.json";
		}
		public string Subscriptions()
		{
			return $@"{_prefix}subscriptions.template.json";
		}
		public string Certificates()
		{
			return $@"{_prefix}certificates.template.json";
		}

		public string ApiInitial(string apiName)
		{
			return $@"{_prefix}{apiName}-initial.api.template.json";
		}

		public string ApiSubsequent(string apiName)
		{
			return $@"{_prefix}{apiName}-subsequent.api.template.json";
		}
	}
}
