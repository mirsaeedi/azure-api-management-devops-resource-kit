using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.Models;
using Apim.DevOps.Toolkit;
using Apim.DevOps.Toolkit.CommandLine;
using System;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
	public class CreateCommand
	{
		public async Task Process(CommandLineOption option)
		{
			await LoadGlobalVariables(option);

			var creatorConfig = await GetCreatorConfig(option);

			await CreateTemplates(creatorConfig);
		}

		private async Task CreateTemplates(DeploymentDefinition creatorConfig)
		{
			var armTemplateCreator = new ArmTemplateCreator(creatorConfig);
			await armTemplateCreator.Create();
		}

		private async Task<DeploymentDefinition> GetCreatorConfig(CommandLineOption option)
		{
			var fileReader = new FileReader();

			var creatorConfig = await fileReader.GetCreatorConfigFromYaml(option.YamlConfigPath);

			creatorConfig.PrefixFileName = option.FileNamePrefix;
			creatorConfig.MasterTemplateName = option.MasterFileName;

			return creatorConfig;
		}

		private async Task LoadGlobalVariables(CommandLineOption option)
		{
			await VariableReplacer.Instance.LoadFromFile(option.VariableFilePath);

			VariableReplacer.Instance.Load(option.VariableString);

			if (option.PrintVariables)
			{
				var keyValuePairs = VariableReplacer.Instance.GetVariables();

				foreach (var kv	 in keyValuePairs)
				{
					Console.WriteLine($"{kv.Key}={kv.Value}");
				}
			}
		}
	}
}
