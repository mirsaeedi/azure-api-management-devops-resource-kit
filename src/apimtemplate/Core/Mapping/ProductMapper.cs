using Apim.DevOps.Toolkit.ApimEntities.Product;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public class ProductMapper : IMapper
	{
		public void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<ProductDeploymentDefinition, ProductsProperties>()
				.ForMember(dst => dst.ApprovalRequired, opt => opt.MapFrom(src => src.SubscriptionRequired ? src.ApprovalRequired : null))
				.ForMember(dst => dst.SubscriptionsLimit, opt => opt.MapFrom(src => src.SubscriptionRequired ? src.SubscriptionsLimit : null));
		}
	}
}
