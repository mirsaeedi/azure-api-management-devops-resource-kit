using Apim.DevOps.Toolkit.ApimEntities.Product;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure;
using Apim.DevOps.Toolkit.Extensions;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public class ProductPolicyMapper: IMapper
	{
		public void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<ProductDeploymentDefinition, ProductPolicyProperties>()
				.ConvertUsing((product, _) =>
				{
					var fileReader = new FileReader();
					var policy = product.Policy;
					var isUrl = policy.IsUri(out var uri);

					return new ProductPolicyProperties()
					{
						Format = isUrl ? "rawxml-link" : "rawxml",
						Value = isUrl ? policy : fileReader.RetrieveFileContentsAsync(policy).Result,
					};
				});
		}
	}
}
