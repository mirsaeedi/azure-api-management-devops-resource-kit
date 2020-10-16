using Apim.DevOps.Toolkit.ApimEntities.User;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public class UserMapper: IMapper
	{
		public void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<UserDeploymentDefinition, UserProperties>();
		}
	}
}
