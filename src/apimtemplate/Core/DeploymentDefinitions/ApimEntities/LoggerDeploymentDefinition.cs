using Apim.DevOps.Toolkit.ApimEntities.Logger;
using System;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.ApimEntities
{
	public class LoggerDeploymentDefinition : EntityDeploymentDefinition
	{
		public string Name { get; set; }

		public string LoggerType { get; set; }

		public string Description { get; set; }

		public LoggerCredentials Credentials { get; set; }

		public bool IsBuffered { get; set; }

		public string ResourceId { get; set; }

		public override IEnumerable<string> Dependencies() => Array.Empty<string>();
	}
}