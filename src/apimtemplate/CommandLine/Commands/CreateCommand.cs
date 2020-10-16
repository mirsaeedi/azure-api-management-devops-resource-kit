using Apim.DevOps.Toolkit.Core.Infrastructure;
using Apim.DevOps.Toolkit.Core.ArmTemplates;
using Apim.DevOps.Toolkit.Core.Variables;
using AutoMapper;
using System;
using System.Threading.Tasks;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using System.IO;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.CommandLine.Commands
{
	public class CreateCommand
	{
		private static readonly FileReader _fileReader = new FileReader();
		private IMapper mapper;

		public CreateCommand(IMapper mapper)
		{
			this.mapper = mapper;
		}

		public async Task ProcessAsync(CommandLineOption option)
		{
			await LoadGlobalVariables(option);

			var deploymentDefinition = await GetDeploymentDefinitionAsync(option);

			var resources = new ArmTemplateCreator(deploymentDefinition, mapper).Create();

			await GenerateArmTemplateFile(resources, deploymentDefinition);
		}


		private async Task GenerateArmTemplateFile(List<ArmTemplateResource> resources, DeploymentDefinition deploymentDefinition)
		{
			var fileGenerator = new ArmTemplateFileGenerator(deploymentDefinition.OutputLocation,
				deploymentDefinition.MasterTemplateName,
				deploymentDefinition.PrefixFileName,
				deploymentDefinition.ApimServiceName);

			await fileGenerator.Save(resources);
		}

		private async Task<DeploymentDefinition> GetDeploymentDefinitionAsync(CommandLineOption option)
		{
			var deploymentDefinition = await AggregateDeploymentDefinitionsAsync(option);

			deploymentDefinition.PrefixFileName = option.FileNamePrefix;
			deploymentDefinition.MasterTemplateName = option.MasterFileName;
			deploymentDefinition.OutputLocation = option.OutputPath;

			foreach (var productDeploymentDefinition in deploymentDefinition.Products)
			{
				productDeploymentDefinition.Root = deploymentDefinition;
			}

			foreach (var apiDeploymentDefinition in deploymentDefinition.Apis)
			{
				apiDeploymentDefinition.Root = deploymentDefinition;
			}

			return deploymentDefinition;
		}

		private async Task<DeploymentDefinition> AggregateDeploymentDefinitionsAsync(CommandLineOption option)
		{
			var deploymentDefinition = default(DeploymentDefinition);

			if (Directory.Exists(option.YamlConfigPath))
			{
				var fileDefinitionPaths = Directory.GetFiles(option.YamlConfigPath, "*.yml", new EnumerationOptions
				{
					RecurseSubdirectories = true
				});

				deploymentDefinition = new DeploymentDefinition();

				foreach (var fileDefinitionPath in fileDefinitionPaths)
				{
					var individualDefinition = await _fileReader.GetDeploymentDefinitionFromYaml(fileDefinitionPath);
					deploymentDefinition = deploymentDefinition.MergeWith(individualDefinition);
				}
			}
			else
			{
				deploymentDefinition = await _fileReader.GetDeploymentDefinitionFromYaml(option.YamlConfigPath);
			}

			return deploymentDefinition;
		}

		private async Task LoadGlobalVariables(CommandLineOption option)
		{
			await VariableReplacer.Instance.LoadFromFile(option.VariableFilePath);
			VariableReplacer.Instance.LoadFromString(option.VariableString);

			if (option.PrintVariables)
			{
				foreach (var variable in VariableReplacer.Instance.Variables)
				{
					Console.WriteLine($"variable is loaded: {variable.Key}={variable.Value}");
				}
			}
		}
	}
}
