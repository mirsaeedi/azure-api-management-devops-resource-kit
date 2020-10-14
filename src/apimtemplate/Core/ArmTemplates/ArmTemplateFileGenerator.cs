using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Apim.DevOps.Toolkit.Core.Infrastructure;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates
{
	public class ArmTemplateFileGenerator
	{
		private static readonly string ApimServiceNameProperyName = "ApimServiceName";
		private readonly FileWriter _fileWriter = new FileWriter();
		private readonly string _masterFileName;
		private readonly string _fileNamePrefix;
		private readonly string _apimServiceName;
		private readonly string _outputLocation;

		public ArmTemplateFileGenerator(string outputLocation, string masterFileName, string fileNamePrefix, string apimServiceName)
		{
			_masterFileName = string.IsNullOrEmpty(masterFileName) ? "apim_deploy.template.json" : masterFileName;
			_fileNamePrefix = string.IsNullOrEmpty(fileNamePrefix) ? string.Empty : fileNamePrefix;
			_apimServiceName = apimServiceName;
			_outputLocation = outputLocation;
		}

		public async Task Save(List<ArmTemplateResource> resources)
		{
			Console.WriteLine("Creating deploy template");
			Console.WriteLine("------------------------------------------");

			var masterTemplate = CreateDeploymentTemplate(resources);
			await SaveTemplateAsync($"{_fileNamePrefix}{_masterFileName}", masterTemplate);

			var templateParameters = CreateParameterTemplate();
			await SaveTemplateAsync($"{_fileNamePrefix}parameters.json", templateParameters);

			Console.WriteLine("Templates written to output location");
		}

		private Task SaveTemplateAsync(string fileName, ArmTemplate template)
		{
			var path = Path.Combine(_outputLocation, fileName);
			return _fileWriter.WriteJsonAsync(template, path);
		}

		private ArmTemplate CreateDeploymentTemplate(IEnumerable<ArmTemplateResource> resources)
		{
			var template = new ArmTemplate();

			template.AddParameter(ApimServiceNameProperyName, new ArmTemplateParameter
			{
				Metadata = new ArmTemplateParameterMetadata
				{
					Description = "Name of the API Management"
				},
				Type = "string"
			});

			template.AddResources(resources);

			return template;
		}

		private ArmTemplate CreateParameterTemplate()
		{
			var template = new ArmTemplate();

			template.AddParameter(ApimServiceNameProperyName, new ArmTemplateParameter()
			{
				Value = _apimServiceName
			});

			return template;
		}
	}
}
