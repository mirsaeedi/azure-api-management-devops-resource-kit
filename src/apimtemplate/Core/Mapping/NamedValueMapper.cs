using Apim.DevOps.Toolkit.ApimEntities.NamedValues;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public class NamedValueMapper: IMapper
	{
		public void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<NamedValueDeploymentDefinition, NamedValueProperties>();
		}
	}
}
