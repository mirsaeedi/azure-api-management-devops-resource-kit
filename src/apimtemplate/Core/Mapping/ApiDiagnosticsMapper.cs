using Apim.DevOps.Toolkit.ApimEntities.Api.Diagnostics;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public class ApiDiagnosticsMapper : IMapper
	{
		public void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<ApiDeploymentDefinition, ApiDiagnosticsProperties>()
				.ConvertUsing((apiDeploymentDefinition, _) =>
				{
					return apiDeploymentDefinition.Diagnostics is null ? null : new ApiDiagnosticsProperties
					{
						Backend = apiDeploymentDefinition.Diagnostics.Backend,
						AlwaysLog = apiDeploymentDefinition.Diagnostics.AlwaysLog,
						EnableHttpCorrelationHeaders = apiDeploymentDefinition.Diagnostics.EnableHttpCorrelationHeaders,
						Frontend = apiDeploymentDefinition.Diagnostics.Frontend,
						HttpCorrelationProtocol = apiDeploymentDefinition.Diagnostics.HttpCorrelationProtocol,
						LogClientIp = apiDeploymentDefinition.Diagnostics.LogClientIp,
						LoggerId = $"[resourceId('{ResourceType.Logger}', parameters('ApimServiceName'), '{apiDeploymentDefinition.Diagnostics.LoggerId}')]",
						Name = apiDeploymentDefinition.Diagnostics.Name,
						Sampling = apiDeploymentDefinition.Diagnostics.Sampling,
						Verbosity = apiDeploymentDefinition.Diagnostics.Verbosity
					};
				});
		}
	}
}