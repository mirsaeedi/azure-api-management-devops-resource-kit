using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.Configuration
{
	public class DeploymentDefinition
	{
		public string Version { get; set; }
		public string ApimServiceName { get; set; }

		/// <summary>
		/// local path or url to global policy
		/// </summary>
		public string Policy { get; set; }
		public IEnumerable<ApiVersionSetDeploymentDefinition> ApiVersionSets { get; set; }
		public IEnumerable<ApiDeploymentDefinition> Apis { get; set; }
		public IEnumerable<CertificateDeploymentDefinition> Certificates { get; set; }
		public IEnumerable<SubscriptionDeploymentDefinition> Subscriptions { get; set; }
		public IEnumerable<UserDeploymentDefinition> Users { get; set; }
		public IEnumerable<ProductDeploymentDefinition> Products { get; set; }
		public IEnumerable<TagDeploymentDefinition> Tags { get; set; }
		public IEnumerable<LoggerDeploymentDefinition> Loggers { get; set; }
		public IEnumerable<AuthorizationServerDeploymentDefinition> AuthorizationServers { get; set; }
		public IEnumerable<BackendDeploymentDefinition> Backends { get; set; }
		public string OutputLocation { get; set; }
		public string LinkedTemplatesBaseUrl { get; set; }
		public string LinkedTemplatesUrlQueryString { get; set; }
		public string PrefixFileName { get; set; }
		public string MasterTemplateName { get; set; }

		public bool HasCertificates() => Certificates != null && Certificates.Count() > 0;
	}
}