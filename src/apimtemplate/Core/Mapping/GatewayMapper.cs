using Apim.DevOps.Toolkit.ApimEntities.Gateway;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
  public class GatewayMapper : IMapper
  {
    public void Map(IMapperConfigurationExpression cfg)
    {
      cfg.CreateMap<GatewayDeploymentDefinition, GatewayProperties>();
    }
  }
}
