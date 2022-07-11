using Apim.DevOps.Toolkit.ApimEntities.Backend;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public class BackendMapper: IMapper
	{
		public void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<BackendDeploymentDefinition, BackendProperties>()
				.ForMember(dst => dst.Properties, opt => opt.MapFrom(src => src.Properties))
				.ForPath(dst => dst.Properties.ServiceFabricCluster.ClientCertificateId, 
						opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Properties.ServiceFabricCluster.ClientCertificateId) 
						? null 
						: $"[resourceId('{ResourceType.Certificate}', parameters('ApimServiceName'), '{src.Properties.ServiceFabricCluster.ClientCertificateId}')]"));
		}
	}
}
