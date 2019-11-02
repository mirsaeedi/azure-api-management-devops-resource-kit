using McMaster.Extensions.CommandLineUtils;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.Models;
using System.Linq;
using Apim.DevOps.Toolkit;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class CreateCommand : CommandLineApplication
    {
        public CreateCommand()
        {
            Name = GlobalConstants.CreateName;
            Description = GlobalConstants.CreateDescription;

            var configFile = Option("--configFile <configFile>", "Config YAML file location", CommandOptionType.SingleValue).IsRequired();

            var replacementFile =  Option("--replacementFile <replacementFile>", "replacement file location", CommandOptionType.SingleValue);

            var replacementVars = Option("--replacementVars <replacementVars>", "replacement variables semicolon seprated", CommandOptionType.SingleValue);

            var prefixFileName = Option("--prefix <prefix>", "prefix of generated files", CommandOptionType.SingleValue);

			var masterFileName = Option("--masterFileName <linkedFileName>", "name of the master template", CommandOptionType.SingleValue);

			this.HelpOption();

            this.OnExecuteAsync(async (cancellationToken) =>
            {
                var creatorConfig = await GetCreatorConfig(configFile, replacementFile, replacementVars, prefixFileName, masterFileName);

                var isConfigCreatorValid = IsCreatorConfigValid(creatorConfig);

                if (!isConfigCreatorValid)
                {
                    return;
                }

                var armTemplateCreator = new ArmTemplateCreator(creatorConfig);
                await armTemplateCreator.Create();
            });
        }

        private bool IsCreatorConfigValid(CreatorConfig creatorConfig)
        {
            CreatorConfigurationValidator creatorConfigurationValidator = new CreatorConfigurationValidator(this);
            bool isValidCreatorConfig = creatorConfigurationValidator.Validate(creatorConfig);
            return isValidCreatorConfig;
        }

        private async Task<CreatorConfig> GetCreatorConfig(CommandOption configFile, 
			CommandOption replacementFile, 
			CommandOption replacementVars, 
			CommandOption prefixFileName, 
			CommandOption masterTemplateName)
        {
			var fileReader = new FileReader();

			await VariableReplacer.Instance.LoadFromFile(replacementFile.Value());

			VariableReplacer.Instance.Load(replacementVars.Value());

            var creatorConfig = await fileReader.GetCreatorConfigFromYaml(configFile.Value());

            creatorConfig.PrefixFileName = prefixFileName.Value();

			creatorConfig.MasterTemplateName =  masterTemplateName.Value();

			return creatorConfig;
        }
    }
}
