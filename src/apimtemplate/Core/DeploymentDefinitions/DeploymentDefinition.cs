using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.ApimEntities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions
{
	public class DeploymentDefinition
	{
		public string Version { get; set; }
		public string ApimServiceName { get; set; }

		/// <summary>
		/// local path or url to global policy
		/// </summary>
		public string Policy { get; set; }

		public IEnumerable<ApiVersionSetDeploymentDefinition> ApiVersionSets { get; set; } = Array.Empty<ApiVersionSetDeploymentDefinition>();

		public IEnumerable<ApiDeploymentDefinition> Apis { get; set; } = Array.Empty<ApiDeploymentDefinition>();

		public IEnumerable<CertificateDeploymentDefinition> Certificates { get; set; } = Array.Empty<CertificateDeploymentDefinition>();

		public IEnumerable<SubscriptionDeploymentDefinition> Subscriptions { get; set; } = Array.Empty<SubscriptionDeploymentDefinition>();

		public IEnumerable<UserDeploymentDefinition> Users { get; set; } = Array.Empty<UserDeploymentDefinition>();

		public IEnumerable<ProductDeploymentDefinition> Products { get; set; } = Array.Empty<ProductDeploymentDefinition>();

		public IEnumerable<TagDeploymentDefinition> Tags { get; set; } = Array.Empty<TagDeploymentDefinition>();

		public IEnumerable<LoggerDeploymentDefinition> Loggers { get; set; } = Array.Empty<LoggerDeploymentDefinition>();

		public IEnumerable<AuthorizationServerDeploymentDefinition> AuthorizationServers { get; set; } = Array.Empty<AuthorizationServerDeploymentDefinition>();

		public IEnumerable<BackendDeploymentDefinition> Backends { get; set; } = Array.Empty<BackendDeploymentDefinition>();

		public string OutputLocation { get; set; }

		public string PrefixFileName { get; set; }

		public string MasterTemplateName { get; set; }

		public bool HasCertificates() => Certificates != null && Certificates.Count() > 0;
	}
}