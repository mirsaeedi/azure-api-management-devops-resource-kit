using Apim.DevOps.Toolkit.ApimEntities.Backend;
using System;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities
{
	public class BackendDeploymentDefinition : EntityDeploymentDefinition
	{
		public string Name { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public string ResourceId { get; set; }

		public Properties Properties { get; set; }

		public Credentials Credentials { get; set; }

		public Proxy Proxy { get; set; }

		public Tls Tls { get; set; }

		public string Url { get; set; }

		public string Protocol { get; set; }

		public override IEnumerable<string> Dependencies() => Array.Empty<string>();
	}
}