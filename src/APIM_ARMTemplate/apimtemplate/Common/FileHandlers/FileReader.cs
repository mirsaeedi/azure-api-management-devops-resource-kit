using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using Apim.DevOps.Toolkit.Extensions;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class FileReader
    {
        private static HttpClient _httpClient = new HttpClient();

        public async Task<string[]> GetReplacementVariablesFromYaml(string replacementVariablesFilePath)
        {
            if (string.IsNullOrEmpty(replacementVariablesFilePath))
            {
                return new string[0];
            }

            var content = await RetrieveFileContentsAsync(replacementVariablesFilePath);
            var deserializer = new Deserializer();
            var replacementVariables = deserializer.Deserialize<string[]>(content);

            return replacementVariables;
        }

        public async Task<CreatorConfig> GetCreatorConfigFromYaml(string configFilePath, IEnumerable<string> replacementVariables)
        {
            var content = await RetrieveFileContentsAsync(configFilePath);

            foreach (var replacementVariable in replacementVariables)
            {
                var keyVal = replacementVariable.CreateReplacementKeyValue();

                content = content.Replace(keyVal.Key, keyVal.Value);
            }

            return GetCreatorConfig(content);
        }

        private static CreatorConfig GetCreatorConfig(string yamlContent)
        {
            var deserializer = new Deserializer();
            object deserializedYaml = deserializer.Deserialize<object>(yamlContent);
           
            var jsonSerializer = new JsonSerializer();

            using (var writer = new StringWriter())
            {
                jsonSerializer.Serialize(writer, deserializedYaml);
                string jsonText = writer.ToString();
                var yamlObject = JsonConvert.DeserializeObject<CreatorConfig>(jsonText);
                return yamlObject;
            }
        }

        public async Task<string> RetrieveFileContentsAsync(string fileLocation)
        {
            var isUrl = fileLocation.IsUri(out var uriResult);

            if (!isUrl)
            {
                return File.ReadAllText(fileLocation);
            }
            
            var response = await _httpClient.GetAsync(uriResult);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Unable to fetch remote file - {fileLocation}");
            }

            return await response.Content.ReadAsStringAsync();
        }

    }
}
