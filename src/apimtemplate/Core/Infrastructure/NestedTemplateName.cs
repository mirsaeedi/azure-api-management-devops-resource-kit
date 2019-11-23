namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
	public class NestedTemplateName
	{
		public NestedTemplateName()
		{
		}

        public string ApiVersionSets()
		{
			return "versionSetTemplate";
		}
		public string AuthorizationServers()
		{
			return "authorizationServersTemplate";
		}
		public string Backends()
		{
			return "backendsTemplate";
		}
		public string GlobalServicePolicy()
		{
			return "globalServicePolicyTemplate";
		}
		public string Loggers()
		{
			return "loggersTemplate";
		}
	
		public string Products()
		{
			return "productsTemplate";
		}
		public string Tags()
		{
			return "tagsTemplate";
		}
		
		public string Users()
		{
			return "usersTemplate";
		}
		public string Subscriptions()
		{
			return "subscriptionTemplate";
		}
		public string Certificates()
		{
			return "certificateTemplate";
		}

		public string ApiInitial(string apiName)
		{
			return $@"{apiName}-InitialAPITemplate";
		}

		public string ApiSubsequent(string apiName)
		{
			return $@"{apiName}-SubsequentAPITemplate";
		}
	}
}
