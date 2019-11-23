using System.Collections.Generic;
using Apim.DevOps.Toolkit.ArmTemplates;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
	public class DeploymentDefinition
	{
		public string Version { get; set; }
		public string ApimServiceName { get; set; }
		
		/// <summary>
		/// local path or url to global policy
		/// </summary>
		public string Policy { get; set; }
		public List<ApiVersionSetDeploymentDefinition> ApiVersionSets { get; set; }
		public List<ApiDeploymentDefinition> Apis { get; set; }
		public List<CertificateDeploymentDefinition> Certificates { get; set; }
		public List<SubscriptionDeploymentDefinition> Subscriptions { get; set; }
		public List<UserDeploymentDefinition> Users { get; set; }
		public List<ProductDeploymentDefinition> Products { get; set; }
		public List<TagDeploymentDefinition> Tags { get; set; }
		public List<LoggerDeploymentDefinition> Loggers { get; set; }
		public List<AuthorizationServerProperties> AuthorizationServers { get; set; }
		public List<BackendProperties> Backends { get; set; }
		public string OutputLocation { get; set; }
		public string LinkedTemplatesBaseUrl { get; set; }
		public string LinkedTemplatesUrlQueryString { get; set; }
		public string PrefixFileName { get; set; }
		public string MasterTemplateName { get; set; }
	}

}