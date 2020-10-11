using System;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.ApimEntities
{
	public class ApiVersionSetDeploymentDefinition : EntityDeploymentDefinition
	{
		/// <summary>
		/// The Id of the Api Version Set
		/// </summary>
		public string Name { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public string VersionQueryName { get; set; }

		public string VersionHeaderName { get; set; }

		public string VersioningScheme { get; set; }

		public override IEnumerable<string> Dependencies() => Array.Empty<string>();
	}
}