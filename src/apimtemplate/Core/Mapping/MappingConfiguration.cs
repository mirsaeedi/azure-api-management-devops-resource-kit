using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public static class MappingConfiguration
	{
		public static IMapper Map()
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.AllowNullCollections = true;
				cfg.AllowNullDestinationValues = true;

				ApiPolicyMapper.Map(cfg);
				ApiMapper.Map(cfg);
				PolicyMapper.Map(cfg);
				ApiVersionSetMapper.Map(cfg);
				AuthorizationServerMapper.Map(cfg);
				BackendMapper.Map(cfg);
				CertificateMapper.Map(cfg);
				LoggerMapper.Map(cfg);
				ProductMapper.Map(cfg);
				ProductPolicyMapper.Map(cfg);
				SubscriptionMapper.Map(cfg);
				TagMapper.Map(cfg);
				UserMapper.Map(cfg);
			});

			return config.CreateMapper();
		}
	}
}
