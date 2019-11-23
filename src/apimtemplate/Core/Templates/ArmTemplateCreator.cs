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
		private TemplateFileName _templateFileNames;

		public ArmTemplateCreator(DeploymentDefinition creatorConfig)
		{
			_creatorConfig = creatorConfig;
			_fileWriter = new FileWriter();
			_templateFileNames = new TemplateFileName(_creatorConfig.PrefixFileName, _creatorConfig.MasterTemplateName);
		}

		public async Task Create()
		{
			Console.WriteLine("Creating global service policy template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<PolicyTemplateCreator>(_templateFileNames.GlobalServicePolicy(), c => c.Policy != null);

			Console.WriteLine("Creating api version sets template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<APIVersionSetTemplateCreator>(_templateFileNames.ApiVersionSets(), c => c.ApiVersionSets != null);

			Console.WriteLine("Creating certificates template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<CertificateTemplateCreator>(_templateFileNames.Certificates(), c => c.Certificates != null);

			Console.WriteLine("Creating users template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<UserTemplateCreator>(_templateFileNames.Users(), c => c.Users != null);

			Console.WriteLine("Creating subscriptions template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<SubscriptionTemplateCreator>(_templateFileNames.Subscriptions(), c => c.Subscriptions != null);

			Console.WriteLine("Creating products template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<ProductTemplateCreator>(_templateFileNames.Products(), c => c.Products != null);

			Console.WriteLine("Creating tags template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<TagTemplateCreator>(_templateFileNames.Tags(), c => c.Tags != null);

			Console.WriteLine("Creating loggers template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<LoggerTemplateCreator>(_templateFileNames.Loggers(), c => c.Loggers != null);

			Console.WriteLine("Creating backeds template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<CertificateTemplateCreator>(_templateFileNames.Backends(), c => c.Backends != null);

			Console.WriteLine("Creating authorization servers template");
			Console.WriteLine("------------------------------------------");
			await SaveTemplate<AuthorizationServerTemplateCreator>(_templateFileNames.AuthorizationServers(), c => c.AuthorizationServers != null);

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
			var masterTemplateCreator = new MasterTemplateCreator(_templateFileNames);

			var masterTemplate = await masterTemplateCreator.Create(_creatorConfig);
			SaveTemplate(_templateFileNames.LinkedMaster(), masterTemplate); //TODO

			var templateParameters = masterTemplateCreator.CreateMasterTemplateParameterValues(_creatorConfig);
			SaveTemplate(_templateFileNames.Parameters(), templateParameters); //TODO
		}

		public async Task SaveApiTemplates()
		{
			var apiTemplateCreator = new ApiTemplateCreator(_creatorConfig.Products, _creatorConfig.Tags);

			foreach (var apiConfiguration in _creatorConfig.Apis)
			{
				var apiTemplates = await apiTemplateCreator.CreateApiTemplatesAsync(apiConfiguration);

				var path = Path.Combine(_creatorConfig.OutputLocation, _templateFileNames.ApiInitial(apiConfiguration.Name));
				SaveTemplate(path, apiTemplates.InitialApiTemplate);


				path = Path.Combine(_creatorConfig.OutputLocation, _templateFileNames.ApiSubsequent(apiConfiguration.Name));
				SaveTemplate(path, apiTemplates.InitialApiTemplate);
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
