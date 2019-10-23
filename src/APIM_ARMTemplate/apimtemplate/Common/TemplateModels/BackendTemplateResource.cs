
namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class BackendTemplateResource : TemplateResource<BackendTemplateProperties>
    {
        public override string Type => ResourceType.Backend;
    }

    public class BackendTemplateProperties
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ResourceId { get; set; }
        public Properties Properties { get; set; }
        public Credentials Credentials { get; set; }
        public Proxy Proxy { get; set; }
        public Tls Tls { get; set; }
        public string Url { get; set; }
        public string Protocol { get; set; }
    }

    public class Properties
    {
        public ServiceFabricCluster ServiceFabricCluster { get; set; }
    }

    public class ServiceFabricCluster
    {
        public string ClientCertificatethumbprint { get; set; }
        public int MaxPartitionResolutionRetries { get; set; }
        public string[] ManagementEndpoints { get; set; }
        public string[] ServerCertificateThumbprints { get; set; }
        public ServerX509Names[] ServerX509Names { get; set; }
    }

    public class ServerX509Names
    {
        public string Name { get; set; }
        public string issuerCertificateThumbprint { get; set; }
    }

    public class Credentials
    {
        public string[] Certificate { get; set; }
        public object Query { get; set; }
        public object Header { get; set; }
        public CredentialsAuthorization Authorization { get; set; }
    }

    public class CredentialsAuthorization
    {
        public string Scheme { get; set; }
        public string Parameter { get; set; }
    }

    public class Proxy
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Tls
    {
        public bool ValidateCertificateChain { get; set; }
        public bool ValidateCertificateName { get; set; }
    }
}
