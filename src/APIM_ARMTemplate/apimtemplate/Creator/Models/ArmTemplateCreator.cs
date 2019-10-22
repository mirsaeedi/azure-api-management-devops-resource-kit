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

        public void Create()
        {
            var fileNameGenerator = new FileNameGenerator();
            var fileNames = fileNameGenerator.GenerateFileNames();

            //_productTemplateCreator = new ProductTemplateCreator(_policyTemplateCreator);
            _masterTemplateCreator = new MasterTemplateCreator(new FileReader());

            SaveTemplate<PolicyTemplateCreator>(fileNames.globalServicePolicy);
            SaveTemplate<APIVersionSetTemplateCreator>(fileNames.apiVersionSets);
            SaveTemplate<LoggerTemplateCreator>(fileNames.loggers);
            SaveTemplate<BackendTemplateCreator>(fileNames.backends);
            SaveTemplate<AuthorizationServerTemplateCreator>(fileNames.authorizationServers);
            SaveTemplate<ProductTemplateCreator>(fileNames.products);
            
            //SaveTemplate<MasterTemplateCreator>(fileNames.products);

            // create parameters file
            Template templateParameters = _masterTemplateCreator.CreateMasterTemplateParameterValues(_creatorConfig);

            // write templates to outputLocation
            if (creatorConfig.linked == true)
            {
                // create linked master template
                Template masterTemplate = _masterTemplateCreator.Create(_creatorConfig);
                fileWriter.WriteJSONToFile(masterTemplate, String.Concat(creatorConfig.outputLocation, fileNames.linkedMaster));
            }



            // write parameters to outputLocation
            fileWriter.WriteJSONToFile(templateParameters, String.Concat(creatorConfig.outputLocation, fileNames.parameters));
            Console.WriteLine("Templates written to output location");
        }

        public async Task CreateApiTemplates()
        {
            Console.WriteLine("Creating API templates");
            Console.WriteLine("------------------------------------------");

            var apiInformation = new List<LinkedMasterTemplateAPIInformation>();
            var apiTemplateCreator = new ApiTemplateCreator(_fileReader);

            foreach (var api in _creatorConfig.apis)
            {
                var apiTemplates = await apiTemplateCreator.CreateAPITemplatesAsync(api);

                foreach (var apiTemplate in apiTemplates)
                {
                    var apiResource = apiTemplate.resources.FirstOrDefault(resource => resource.type == ResourceType.Api) as ApiTemplateResource;
                    var apiConfiguration = _creatorConfig.apis.FirstOrDefault(api => apiResource.name.Contains(api.name, StringComparison.Ordinal));


                    // if the api version is not null the api is split into multiple templates. If the template is split and the content value has been set, then the template is for a subsequent api

                    string apiFileName = fileNameGenerator.GenerateCreatorAPIFileName(providedAPIConfiguration.name, apiTemplateCreator.isSplitAPI(providedAPIConfiguration), apiResource.properties.value == null, creatorConfig.apimServiceName);

                    fileWriter.WriteJSONToFile(apiTemplate, String.Concat(creatorConfig.outputLocation, apiFileName));
                }
            }
        }

        private void SaveTemplate<TemplateCreator>(string fileName,params object[] constructorArgs) where TemplateCreator:ITemplateCreator
        {
            var templateCreator = (TemplateCreator)Activator.CreateInstance(typeof(TemplateCreator), constructorArgs);

            Console.WriteLine("Creating global service policy template");
            Console.WriteLine("------------------------------------------");
            var template = _creatorConfig.policy != null ? templateCreator.Create(_creatorConfig) : null;

            var fileWriter = new FileWriter();
            var path = Path.Combine(_creatorConfig.outputLocation, fileName);

            fileWriter.WriteJSONToFile(template, path);
        }
    }
}
