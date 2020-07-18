using Apim.DevOps.Toolkit.ApimEntities.Backend;
using Apim.DevOps.Toolkit.Core.Configuration;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public static class BackendMapper
	{
		internal static void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<BackendDeploymentDefinition, BackendProperties>();
		}
	}
}
