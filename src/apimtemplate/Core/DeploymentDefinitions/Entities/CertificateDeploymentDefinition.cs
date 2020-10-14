using System;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities
{
	public class CertificateDeploymentDefinition : EntityDeploymentDefinition
	{
		/// <summary>
		/// The Id of the Certificate
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The path to the pfx certificate
		/// </summary>
		public string FilePath { get; set; }

		public string Password { get; set; }

		public override IEnumerable<string> Dependencies() => Array.Empty<string>();
	}
}