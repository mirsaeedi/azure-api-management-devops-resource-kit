using Apim.DevOps.Toolkit.ApimEntities.Subscription;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.ApimEntities;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public static class SubscriptionMapper
	{
		internal static void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<SubscriptionDeploymentDefinition, SubscriptionProperties>();
		}
	}
}
