using Apim.DevOps.Toolkit.ApimEntities.Logger;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.ApimEntities
{
	public class LoggerDeploymentDefinition
	{
		public string Name { get; set; }

		public string LoggerType { get; set; }

		public string Description { get; set; }

		public LoggerCredentials Credentials { get; set; }

		public bool IsBuffered { get; set; }

		public string ResourceId { get; set; }
	}
}