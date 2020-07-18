using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apim.DevOps.Toolkit.Core.Templates
{
	public interface ITemplateCreator
	{
		Task<Template> Create<TResourceDeploymentDefinition, TResourceProperties>(
		   TResourceDeploymentDefinition resourceDeploymentDefinition,
		   Func<TResourceDeploymentDefinition, string> getResourceName, string resourceType, IEnumerable<string> dependsOn = null);

		Task<Template> Create<TResourceDeploymentDefinition, TResourceProperties>(
			IEnumerable<TResourceDeploymentDefinition> resourceDeploymentDefinitions,
			Func<TResourceDeploymentDefinition, string> getResourceName,
			string resourceType, IEnumerable<string> dependsOn = null);
	}
}
