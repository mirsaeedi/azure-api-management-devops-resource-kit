using Apim.DevOps.Toolkit.ApimEntities.AuthotizationServer;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.ApimEntities;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public static class AuthorizationServerMapper
	{
		internal static void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<AuthorizationServerDeploymentDefinition, AuthorizationServerProperties>();
		}
	}
}
