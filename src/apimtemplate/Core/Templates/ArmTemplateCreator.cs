using Apim.Arm.Creator.Creator.TemplateCreators;
using Apim.DevOps.Toolkit.ApimEntities.Tag;
using Apim.DevOps.Toolkit.ArmTemplates;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apim.Arm.Creator.Creator.Models
{
	public class ArmTemplateCreator
	{
		private DeploymentDefinition _creatorConfig;
		private FileWriter _fileWriter;

		public ArmTemplateCreator(DeploymentDefinition creatorConfig)
		{
			_creatorConfig = creatorConfig;
			_fileWriter = new FileWriter();
		}

		public async Task Create()
		{
			var fileNameGenerator = new FileNameGenerator(_creatorConfig.PrefixFileName, _creatorConfig.MasterTemplateName);
			var fileNames = fileNameGenerator.GenerateFileNames();

			Console.WriteLine("Creating global service policy template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<PolicyTemplateCreator>(fileNames.GlobalServicePolicy, c => c.Policy != null);

			Console.WriteLine("Creating api version sets template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<APIVersionSetTemplateCreator>(fileNames.ApiVersionSets, c => c.ApiVersionSets != null);

			Console.WriteLine("Creating products template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<ProductTemplateCreator>(fileNames.Products, c => c.Products != null);

			Console.WriteLine("Creating tags template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<TagTemplateCreator>(fileNames.Tags, c => c.Tags != null);

			Console.WriteLine("Creating loggers template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<LoggerTemplateCreator>(fileNames.Loggers, c => c.Loggers != null);

			Console.WriteLine("Creating backeds template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<BackendTemplateCreator>(fileNames.Backends, c => c.Backends != null);

			Console.WriteLine("Creating authorization servers template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<AuthorizationServerTemplateCreator>(fileNames.AuthorizationServers, c => c.AuthorizationServers != null);

			Console.WriteLine("Creating api templates");
			Console.WriteLine("------------------------------------------");
			await SaveApiTemplates();

			Console.WriteLine("Creating master template");
			Console.WriteLine("------------------------------------------");
			await SaveMasterTemplate();

			Console.WriteLine("Templates written to output location");
		}

		private async Task SaveMasterTemplate()
		{
			var fileNameGenerator = new FileNameGenerator(_creatorConfig.PrefixFileName, _creatorConfig.MasterTemplateName);
			var fileNames = fileNameGenerator.GenerateFileNames();

			var masterTemplateCreator = new MasterTemplateCreator();

			var masterTemplate = await masterTemplateCreator.Create(_creatorConfig);
			SaveTemplate(fileNames.LinkedMaster, masterTemplate); //TODO

			var templateParameters = masterTemplateCreator.CreateMasterTemplateParameterValues(_creatorConfig);
			SaveTemplate(fileNames.Parameters, templateParameters); //TODO
		}

		public async Task SaveApiTemplates()
		{
			var apiTemplateCreator = new ApiTemplateCreator(_creatorConfig.Products, _creatorConfig.Tags);

			foreach (var apiConfiguration in _creatorConfig.Apis)
			{
				var apiTemplates = await apiTemplateCreator.CreateApiTemplatesAsync(apiConfiguration);

				foreach (var apiTemplate in apiTemplates)
				{
					var apiResource = apiTemplate.Resources.FirstOrDefault(resource => resource.Type == ResourceType.Api) as ApiTemplateResource; // todo
					string apiFileName = new FileNameGenerator(_creatorConfig.PrefixFileName, _creatorConfig.MasterTemplateName)
						.GenerateCreatorAPIFileName(apiConfiguration.Name, true, apiResource.Properties.Value != null, _creatorConfig.ApimServiceName);

					var path = Path.Combine(_creatorConfig.OutputLocation, apiFileName);
					SaveTemplate(path, apiTemplate);
				}
			}
		}

		private async Task SaveTemplate<TemplateCreator>(string fileName, Predicate<DeploymentDefinition> hasTemplatePredicate, params object[] constructorArgs) where TemplateCreator : ITemplateCreator
		{
			var templateCreator = (TemplateCreator)Activator.CreateInstance(typeof(TemplateCreator), constructorArgs);

			if (hasTemplatePredicate(_creatorConfig))
			{
				var template = await templateCreator.Create(_creatorConfig);
				SaveTemplate(fileName, template);
			}
		}

		private void SaveTemplate(string fileName, Template template)
		{
			var path = Path.Combine(_creatorConfig.OutputLocation, fileName);
			_fileWriter.WriteJson(template, path);
		}
	}
}
