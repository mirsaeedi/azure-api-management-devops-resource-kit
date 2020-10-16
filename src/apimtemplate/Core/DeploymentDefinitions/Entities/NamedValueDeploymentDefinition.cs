using System;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities
{
	public class NamedValueDeploymentDefinition : EntityDeploymentDefinition
	{
		/// <summary>
		/// The Id of the Named Value
		/// </summary>
		public string Name { get; set; }

		public string DisplayName { get; set; }

		public string Value { get; set; }

		public bool Secret { get; set; }

		public override IEnumerable<string> Dependencies() => Array.Empty<string>();
	}
}