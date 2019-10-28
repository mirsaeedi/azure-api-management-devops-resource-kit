using McMaster.Extensions.CommandLineUtils;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.Models;
using System.Linq;

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
            
            this.HelpOption();

            this.OnExecuteAsync(async (cancellationToken) =>
            {
                var creatorConfig = await GetCreatorConfig(configFile, replacementFile, replacementVars);

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

        private async Task<CreatorConfig> GetCreatorConfig(CommandOption configFile,CommandOption replacementFile, CommandOption replacementVars)
        {
            var fileReader = new FileReader();

            var replacementVariablesFromFile = await fileReader.GetReplacementVariablesFromYaml(replacementFile.Value());

            var replacementVariablesFromCommandLine = !replacementVars.HasValue() ? new string[0] : replacementVars.Value().Split(";");

            var vars = replacementVariablesFromFile.Concat(replacementVariablesFromCommandLine);

            return await fileReader.GetCreatorConfigFromYaml(configFile.Value(), vars);
        }
    }
}
