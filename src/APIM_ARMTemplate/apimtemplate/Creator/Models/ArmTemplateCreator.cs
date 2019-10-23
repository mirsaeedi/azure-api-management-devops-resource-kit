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
        private APIVersionSetTemplateCreator _apiVersionSetTemplateCreator;
        private LoggerTemplateCreator _loggerTemplateCreator;
        private BackendTemplateCreator _backendTemplateCreator;
        private AuthorizationServerTemplateCreator _authorizationServerTemplateCreator;
        private ProductAPITemplateCreator _productAPITemplateCreator;
        private PolicyTemplateCreator _policyTemplateCreator;
        private DiagnosticTemplateCreator _diagnosticTemplateCreator;
        private ReleaseTemplateCreator _releaseTemplateCreator;
        private ProductTemplateCreator _productTemplateCreator;
        private ApiTemplateCreator _apiTemplateCreator;
        private MasterTemplateCreator _masterTemplateCreator;

        public ArmTemplateCreator(CreatorConfig creatorConfig,FileReader fileReader)
        {
            _creatorConfig = creatorConfig;
            _fileReader = fileReader;
        }

        public async Task Create()
        {
            var fileNameGenerator = new FileNameGenerator();
            var fileNames = fileNameGenerator.GenerateFileNames();

            //_productTemplateCreator = new ProductTemplateCreator(_policyTemplateCreator);
            
            SaveTemplate<PolicyTemplateCreator>(fileNames.globalServicePolicy);
            SaveTemplate<APIVersionSetTemplateCreator>(fileNames.apiVersionSets);
            SaveTemplate<LoggerTemplateCreator>(fileNames.loggers);
            SaveTemplate<BackendTemplateCreator>(fileNames.backends);
            SaveTemplate<AuthorizationServerTemplateCreator>(fileNames.authorizationServers);
            SaveTemplate<ProductTemplateCreator>(fileNames.products);

            await SaveApiTemplates();
            SaveMasterTemplate();
            
            Console.WriteLine("Templates written to output location");
        }

        private void SaveMasterTemplate()
        {
            var fileNameGenerator = new FileNameGenerator();
            var fileNames = fileNameGenerator.GenerateFileNames();

            var masterTemplateCreator = new MasterTemplateCreator(new FileReader());

            if (_creatorConfig.linked == true)
            {
                var masterTemplate = masterTemplateCreator.Create(_creatorConfig);
                SaveTemplate(fileNames.linkedMaster, masterTemplate); //TODO
            }

            var templateParameters = _masterTemplateCreator.CreateMasterTemplateParameterValues(_creatorConfig);
            SaveTemplate(fileNames.parameters, templateParameters); //TODO
        }

        public async Task SaveApiTemplates()
        {
            Console.WriteLine("Creating API templates");
            Console.WriteLine("------------------------------------------");

            var apiInformation = new List<LinkedMasterTemplateAPIInformation>();
            var apiTemplateCreator = new ApiTemplateCreator(_fileReader);

            foreach (var apiConfiguration in _creatorConfig.apis)
            {
                var apiTemplates = await apiTemplateCreator.CreateAPITemplatesAsync(apiConfiguration);

                foreach (var apiTemplate in apiTemplates)
                {
                    var apiResource = apiTemplate.resources.FirstOrDefault(resource => resource.Type == ResourceType.Api) as ApiTemplateResource;
                    string apiFileName = new FileNameGenerator().GenerateCreatorAPIFileName(apiConfiguration.name, apiConfiguration.IsSplitApi(), apiResource.properties.value == null, _creatorConfig.apimServiceName);

                    SaveTemplate(apiFileName,apiTemplate);
                }
            }
        }

        private void SaveTemplate<TemplateCreator>(string fileName,params object[] constructorArgs) where TemplateCreator:ITemplateCreator
        {
            var templateCreator = (TemplateCreator)Activator.CreateInstance(typeof(TemplateCreator), constructorArgs);

            Console.WriteLine("Creating global service policy template");
            Console.WriteLine("------------------------------------------");
            var template = _creatorConfig.policy != null ? templateCreator.Create(_creatorConfig) : null;

            SaveTemplate(fileName, template);
        }

        private void SaveTemplate(string fileName, Template template)
        {
            var path = Path.Combine(_creatorConfig.outputLocation, fileName);
            var fileWriter = new FileWriter();
            fileWriter.WriteJson(template, path);
        }
    }
}
