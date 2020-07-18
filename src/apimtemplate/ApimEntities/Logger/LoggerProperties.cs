namespace Apim.DevOps.Toolkit.ApimEntities.Logger
{
	public class LoggerProperties
	{
		public string LoggerType { get; set; }

		public string Description { get; set; }

		public LoggerCredentials Credentials { get; set; }

		public bool IsBuffered { get; set; }

		public string ResourceId { get; set; }
	}
}
