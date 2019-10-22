using McMaster.Extensions.CommandLineUtils;
using System;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class CreateCommand : CommandLineApplication
    {
        public CreateCommand()
        {
            this.Name = GlobalConstants.CreateName;
            this.Description = GlobalConstants.CreateDescription;

            // list command options
            CommandOption configFile = this.Option("--configFile <configFile>", "Config YAML file location", CommandOptionType.SingleValue).IsRequired();

            this.HelpOption();

            this.OnExecuteAsync(async (cancellationToken) =>
            {
                var creatorConfig = await GetCreatorConfig(configFile);

                var isConfigCreatorValid = IsCreatorConfigValid(creatorConfig);

                if (!isConfigCreatorValid)
                {
                    return;
                }
            });
        }

        private bool IsCreatorConfigValid(CreatorConfig creatorConfig)
        {
            CreatorConfigurationValidator creatorConfigurationValidator = new CreatorConfigurationValidator(this);
            bool isValidCreatorConfig = creatorConfigurationValidator.ValidateCreatorConfig(creatorConfig);
            return isValidCreatorConfig;
        }

        private async Task<CreatorConfig> GetCreatorConfig(CommandOption configFile)
        {
            var fileReader = new FileReader();
            return await fileReader.ConvertConfigYAMLToCreatorConfigAsync(configFile.Value());
        }
    }
}
