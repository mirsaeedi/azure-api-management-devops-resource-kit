using Apim.DevOps.Toolkit.ApimEntities.Policy;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using Apim.DevOps.Toolkit.Core.Infrastructure;
using Apim.DevOps.Toolkit.Extensions;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public static class PolicyMapper
	{
		internal static void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<DeploymentDefinition, PolicyProperties>()
				.ConvertUsing((deploymentDefinition, _) =>
				{
					var fileReader = new FileReader();
					var policy = deploymentDefinition.Policy;
					var isUrl = policy.IsUri(out var uri);

					return new PolicyProperties()
					{
						Format = isUrl ? "rawxml-link" : "rawxml",
						Value = isUrl ? policy : fileReader.RetrieveFileContentsAsync(policy).Result,
					};
				});
		}
	}
}
