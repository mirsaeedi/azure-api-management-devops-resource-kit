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
			cfg.CreateMap<BackendDeploymentDefinition, BackendProperties>();
			cfg.CreateMap<ServiceFabricCluster, ServiceFabricCluster>()
				.ForMember(dst => dst.ClientCertificateId, 
					opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.ClientCertificateId) 
						? $"[resourceId('{ResourceType.Certificate}', parameters('ApimServiceName'), '{src.ClientCertificateId}')]" 
						: null));
			cfg.CreateMap<Properties, Properties>()
				.ForMember(dst => dst.ServiceFabricCluster, opt => opt.MapFrom(src => src.ServiceFabricCluster));
		}
	}
}
