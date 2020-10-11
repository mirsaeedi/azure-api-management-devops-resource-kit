using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.ApimEntities
{
	public abstract class EntityDeploymentDefinition
	{
		public abstract IEnumerable<string> Dependencies();
	}
}