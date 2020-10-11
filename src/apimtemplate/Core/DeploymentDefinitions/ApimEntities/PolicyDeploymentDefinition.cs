using System;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.ApimEntities
{
	public class PolicyDeploymentDefinition : EntityDeploymentDefinition
	{
		public string Value { get; set; }

		public string Format { get; set; }

		public override IEnumerable<string> Dependencies() => Array.Empty<string>();
	}
}
