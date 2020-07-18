﻿using Apim.DevOps.Toolkit.ApimEntities.Policy;
using Apim.DevOps.Toolkit.Core.Configuration;
using Apim.DevOps.Toolkit.Core.Infrastructure;
using Apim.DevOps.Toolkit.Extensions;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public static class ProductPolicyMapper
	{
		internal static void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<ProductDeploymentDefinition, PolicyProperties>()
				.ConvertUsing((product, _) =>
				{
					var fileReader = new FileReader();
					var policy = product.Policy;
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
