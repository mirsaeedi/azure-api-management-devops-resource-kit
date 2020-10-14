using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities
{
	public abstract class EntityDeploymentDefinition
	{
		public abstract IEnumerable<string> Dependencies();
	}
}