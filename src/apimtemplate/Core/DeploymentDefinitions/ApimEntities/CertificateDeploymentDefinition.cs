namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.ApimEntities
{
	public class CertificateDeploymentDefinition
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
	}
}