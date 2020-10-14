using Apim.DevOps.Toolkit.ApimEntities.User;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public static class UserMapper
	{
		internal static void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<UserDeploymentDefinition, UserProperties>();
		}
	}
}
