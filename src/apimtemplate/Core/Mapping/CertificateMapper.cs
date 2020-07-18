using Apim.DevOps.Toolkit.ArmTemplates;
using Apim.DevOps.Toolkit.Core.Configuration;
using Apim.DevOps.Toolkit.Core.Infrastructure;
using AutoMapper;

namespace Apim.DevOps.Toolkit.Core.Mapping
{
	public static class CertificateMapper
	{
		internal static void Map(IMapperConfigurationExpression cfg)
		{
			cfg.CreateMap<CertificateDeploymentDefinition, CertificateProperties>()
				.ConvertUsing((certificateDeploymentDefinition, _) =>
				{
					var fileReader = new FileReader();

					return new CertificateProperties
					{
						Data = fileReader.RetrieveFileContentsAsync(certificateDeploymentDefinition.FilePath, convertToBase64: true).Result,
						Password = certificateDeploymentDefinition.Password,
					};
				});
		}
	}
}
