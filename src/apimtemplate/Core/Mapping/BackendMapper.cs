using Apim.DevOps.Toolkit.ApimEntities.Backend;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
    public class BackendMapper : IMapper
    {
        public void Map(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<BackendDeploymentDefinition, BackendProperties>()
                .AfterMap((backendDeploymentDefinition, backendProperties) =>
                {
                    if (backendDeploymentDefinition?.Properties?.ServiceFabricCluster?.ClientCertificateId is not null)
                    {
                        backendProperties.Properties.ServiceFabricCluster.ClientCertificateId = $"[resourceId('{ResourceType.Certificate}', parameters('ApimServiceName'), '{backendDeploymentDefinition.Properties.ServiceFabricCluster.ClientCertificateId}')]";
                    }
                });
        }
    }
}
