using System.Collections.Generic;
using Apim.DevOps.Toolkit.ArmTemplates;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
	public class DeploymentDefinition
	{
		public string Version { get; set; }
		public string ApimServiceName { get; set; }
		// policy file location (local or url)
		public string Policy { get; set; }
		public List<ApiVersionSetDeploymentDefinition> ApiVersionSets { get; set; }
		public List<ApiDeploymentDefinition> Apis { get; set; }
		public List<ProductDeploymentDefinition> Products { get; set; }
		public List<TagDeploymentDefinition> Tags { get; set; }
		public List<LoggerDeploymentDefinition> Loggers { get; set; }
		public List<AuthorizationServerProperties> AuthorizationServers { get; set; }
		public List<BackendProperties> Backends { get; set; }
		public string OutputLocation { get; set; }
		public bool Linked { get; set; }
		public string LinkedTemplatesBaseUrl { get; set; }
		public string LinkedTemplatesUrlQueryString { get; set; }
		public string PrefixFileName { get; set; }
		public string MasterTemplateName { get; set; }
	}

}