namespace Apim.DevOps.Toolkit.Core.Templates
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

		public string ApiVersionSets => $"{_prefix}apiVersionSets.template.json";

		public string AuthorizationServers => $"{_prefix}authorizationServers.template.json";

		public string Backends => $"{_prefix}backends.template.json";

		public string GlobalServicePolicy => $"{_prefix}globalServicePolicy.template.json";

		public string Loggers => $"{_prefix}loggers.template.json";

		public string NamedValues => $"{_prefix}namedValues.template.json";

		public string Products => $"{_prefix}products.template.json";

		public string Tags => $"{_prefix}tags.template.json";

		public string Parameters => $"{_prefix}parameters.json";

		// linked property outputs 1 master template
		public string LinkedMaster => $"{_prefix}{_masterTemplateName}.json";

		public string Users => $"{_prefix}users.template.json";

		public string Subscriptions => $"{_prefix}subscriptions.template.json";

		public string Certificates => $"{_prefix}certificates.template.json";

		public string ApiInitial => $"{_prefix}-initial.api.template.json";

		public string ApiSubsequent => $"{_prefix}-subsequent.api.template.json";
	}
}
