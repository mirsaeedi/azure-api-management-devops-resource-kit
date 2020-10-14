using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates.ResourceCreators
{
	public interface IResourceCreator
	{
		IEnumerable<ArmTemplateResource> Create(DeploymentDefinition deploymentDefinition);
	}
}
