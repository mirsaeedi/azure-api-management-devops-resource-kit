using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities
{
	public abstract class EntityDeploymentDefinition
	{
		public DeploymentDefinition Root { get; set; }

		public abstract IEnumerable<string> Dependencies();
	}
}