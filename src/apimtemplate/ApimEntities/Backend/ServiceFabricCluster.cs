namespace Apim.DevOps.Toolkit.ApimEntities.Backend
{
	public class ServiceFabricCluster
	{
		public string ClientCertificatethumbprint { get; set; }
		public int MaxPartitionResolutionRetries { get; set; }
		public string[] ManagementEndpoints { get; set; }
		public string[] ServerCertificateThumbprints { get; set; }
		public ServerX509Names[] ServerX509Names { get; set; }
	}
}
