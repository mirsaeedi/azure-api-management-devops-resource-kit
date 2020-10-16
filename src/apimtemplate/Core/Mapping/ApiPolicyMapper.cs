using Apim.DevOps.Toolkit.ApimEntities.Api.Policy;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure;
using Apim.DevOps.Toolkit.Extensions;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public class ApiPolicyMapper: IMapper
	{
		public void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<ApiDeploymentDefinition, ApiPolicyProperties>()
				.ConvertUsing((api, _) =>
				{
					var fileReader = new FileReader();
					var policy = api.Policy;
					var isUrl = policy.IsUri(out var uri);

					return new ApiPolicyProperties()
					{
						Format = isUrl ? "rawxml-link" : "rawxml",
						Value = isUrl ? policy : fileReader.RetrieveFileContentsAsync(policy).Result,
					};
				});
		}
	}
}