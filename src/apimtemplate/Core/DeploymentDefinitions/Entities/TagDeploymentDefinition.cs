using System;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities
{
	public class TagDeploymentDefinition : EntityDeploymentDefinition
	{
		/// <summary>
		/// The Id of the tag
		/// </summary>
		public string Name { get; set; }

		public string DisplayName { get; set; }

		public override IEnumerable<string> Dependencies() => Array.Empty<string>();
	}
}