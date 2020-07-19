using Apim.DevOps.Toolkit.ApimEntities.Api;
using Apim.DevOps.Toolkit.Core.Configuration;
using Apim.DevOps.Toolkit.Core.Infrastructure;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public static class ApiInitialMapper
	{
		internal static void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<ApiDeploymentDefinition, ApiInitialProperties>()
				.ConvertUsing((api, _) =>
				{
					var openApiSpecReader = new OpenApiSpecReader(api.OpenApiSpec);
					var format = openApiSpecReader.GetOpenApiFormat().Result;
					var value = openApiSpecReader.GetValue().Result;
					var path = api.Path;

					return new ApiInitialProperties(path, format, value);
				});
		}
	}
}