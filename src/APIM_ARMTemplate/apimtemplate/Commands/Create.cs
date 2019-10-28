using McMaster.Extensions.CommandLineUtils;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.Models;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class CreateCommand : CommandLineApplication
    {
        public CreateCommand()
        {
            Name = GlobalConstants.CreateName;
            Description = GlobalConstants.CreateDescription;

            var configFile = Option("--configFile <configFile>", "Config YAML file location", CommandOptionType.SingleValue).IsRequired();

            this.HelpOption();

            this.OnExecuteAsync(async (cancellationToken) =>
            {
                var creatorConfig = await GetCreatorConfig(configFile);

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

        private async Task<CreatorConfig> GetCreatorConfig(CommandOption configFile)
        {
            var fileReader = new FileReader();
            return await fileReader.GetCreatorConfigFromYaml(configFile.Value());
        }
    }
}
