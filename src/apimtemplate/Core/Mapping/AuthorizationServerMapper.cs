using Apim.DevOps.Toolkit.ApimEntities.AuthotizationServer;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public class AuthorizationServerMapper: IMapper
	{
		public void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<AuthorizationServerDeploymentDefinition, AuthorizationServerProperties>();
		}
	}
}
