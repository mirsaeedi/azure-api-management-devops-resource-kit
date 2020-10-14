using System;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities
{
	public class OperationsDeploymentDefinition: EntityDeploymentDefinition
	{
		/// <summary>
		/// Local path or url to a policy
		/// </summary>
		public string Policy { get; set; }


		public override IEnumerable<string> Dependencies() => Array.Empty<string>();
	}
}