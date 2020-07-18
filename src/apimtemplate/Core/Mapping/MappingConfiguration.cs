using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public static class MappingConfiguration
	{
		public static void Map()
		{
			var config = new MapperConfiguration(cfg =>
			{
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
		}
	}
}
