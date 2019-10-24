using Apim.Arm.Creator.Creator.TemplateCreators;
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
        private CreatorConfig _creatorConfig;
        private FileReader _fileReader;

        public ArmTemplateCreator(CreatorConfig creatorConfig)
        {
            _creatorConfig = creatorConfig;
            _fileReader = new FileReader();
        }

        public async Task Create()
        {
            var fileNameGenerator = new FileNameGenerator();
            var fileNames = fileNameGenerator.GenerateFileNames();

            Console.WriteLine("Creating global service policy template");
            Console.WriteLine("------------------------------------------");
            await SaveTemplate<PolicyTemplateCreator>(fileNames.globalServicePolicy,c=>c.Policy!=null);

            Console.WriteLine("Creating api version sets template");
            Console.WriteLine("------------------------------------------");
            await SaveTemplate<APIVersionSetTemplateCreator>(fileNames.apiVersionSets, c => c.ApiVersionSets != null);

            Console.WriteLine("Creating products template");
            Console.WriteLine("------------------------------------------");
            await SaveTemplate<ProductTemplateCreator>(fileNames.products, c => c.Products != null);

            Console.WriteLine("Creating loggers template");
            Console.WriteLine("------------------------------------------");
            await SaveTemplate<LoggerTemplateCreator>(fileNames.loggers, c => c.Loggers != null);

            Console.WriteLine("Creating backeds template");
            Console.WriteLine("------------------------------------------");
            await SaveTemplate<BackendTemplateCreator>(fileNames.backends, c => c.Backends != null);

            Console.WriteLine("Creating authorization servers template");
            Console.WriteLine("------------------------------------------");
            await SaveTemplate<AuthorizationServerTemplateCreator>(fileNames.authorizationServers, c => c.AuthorizationServers != null);


            Console.WriteLine("Creating api templates");
            Console.WriteLine("------------------------------------------");
            await SaveApiTemplates();

            Console.WriteLine("Creating master template");
            Console.WriteLine("------------------------------------------");
            await SaveMasterTemplate();
            
            Console.WriteLine("Templates written to output location");
        }

        private async Task  SaveMasterTemplate()
        {
            var fileNameGenerator = new FileNameGenerator();
            var fileNames = fileNameGenerator.GenerateFileNames();

            var masterTemplateCreator = new MasterTemplateCreator();

            if (_creatorConfig.Linked == true)
            {
                var masterTemplate = await masterTemplateCreator.Create(_creatorConfig);
                SaveTemplate(fileNames.linkedMaster, masterTemplate); //TODO

                var templateParameters = masterTemplateCreator.CreateMasterTemplateParameterValues(_creatorConfig);
                SaveTemplate(fileNames.parameters, templateParameters); //TODO
            }

        }

        public async Task SaveApiTemplates()
        {
            var apiInformation = new List<LinkedMasterTemplateAPIInformation>();
            var apiTemplateCreator = new ApiTemplateCreator(_fileReader);

            foreach (var apiConfiguration in _creatorConfig.Apis)
            {
                var apiTemplates = await apiTemplateCreator.CreateAPITemplatesAsync(apiConfiguration);

                foreach (var apiTemplate in apiTemplates)
                {
                    var apiResource = apiTemplate.resources.FirstOrDefault(resource => resource.Type == ResourceType.Api) as ApiTemplateResource; // todo
                    string apiFileName = new FileNameGenerator().GenerateCreatorAPIFileName(apiConfiguration.name, apiConfiguration.IsSplitApi(), apiResource.Properties.value != null, _creatorConfig.ApimServiceName);
                    
                    var path = Path.Combine(_creatorConfig.OutputLocation, apiFileName);
                    SaveTemplate(path, apiTemplate);
                }
            }
        }

        private async Task SaveTemplate<TemplateCreator>(string fileName,Predicate<CreatorConfig> hasTemplatePredicate,params object[] constructorArgs) where TemplateCreator:ITemplateCreator
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
            var fileWriter = new FileWriter();
            fileWriter.WriteJson(template, path);
        }
    }
}
