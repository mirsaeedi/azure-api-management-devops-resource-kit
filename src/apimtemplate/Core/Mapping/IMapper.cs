using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	interface IMapper
	{
		void Map(IMapperConfigurationExpression cfg);
	}
}
