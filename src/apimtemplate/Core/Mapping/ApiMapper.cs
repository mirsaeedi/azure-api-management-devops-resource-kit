using Apim.DevOps.Toolkit.ApimEntities.Api;
using Apim.DevOps.Toolkit.Core.Configuration;
using Apim.DevOps.Toolkit.Core.Infrastructure;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using Apim.DevOps.Toolkit.Extensions;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public static class ApiMapper
	{
		internal static void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<ApiDeploymentDefinition, ApiProperties>()
				.ForMember(dst => dst.Path, opt => opt.MapFrom(src => src.Path.Replace("//", "/").TrimStart(new[] { '/' })))
				.ForMember(dst => dst.Format, opt => opt.MapFrom(src => new OpenApiSpecReader(src.OpenApiSpec).GetOpenApiFormat().Result))
				.ForMember(dst => dst.Value, opt => opt.MapFrom(src => new OpenApiSpecReader(src.OpenApiSpec).GetValue(src.DisplayName).Result))
				.ForMember(dst => dst.Protocols, opt => opt.MapFrom(src => src.Protocols.GetItems(new[] { "https" })))
				.ForMember(dst => dst.ApiVersionSetId, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ApiVersionSetId) ? null : $"[resourceId('{ResourceType.ApiVersionSet}', parameters('ApimServiceName'), '{src.ApiVersionSetId}')]"));

		}
	}
}