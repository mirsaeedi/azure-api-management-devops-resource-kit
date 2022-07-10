namespace Apim.DevOps.Toolkit.ApimEntities.Backend
{
	public class Credentials
	{
		public string[] Certificate { get; set; }

		public object Query { get; set; }

		public object Header { get; set; }

		public CredentialsAuthorization Authorization { get; set; }
	}
}
