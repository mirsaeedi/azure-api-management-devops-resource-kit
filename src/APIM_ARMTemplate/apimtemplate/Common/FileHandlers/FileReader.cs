using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class FileReader
    {
        private static HttpClient _httpClient = new HttpClient();
        public async Task<CreatorConfig> ConvertConfigYAMLToCreatorConfigAsync(string configFileLocation)
        {
            var content = await RetrieveFileContentsAsync(configFileLocation);
            return ConvertYamlToJson(content);
        }

        private static CreatorConfig ConvertYamlToJson(string yamlContent)
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

        public string RetrieveLocalFileContents(string fileLocation)
        {
            return File.ReadAllText(fileLocation);
        }

        public async Task<string> RetrieveFileContentsAsync(string fileLocation)
        {
            bool isUrl = Uri.TryCreate(fileLocation, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            
            if (isUrl)
            {
                var response = await _httpClient.GetAsync(uriResult);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Unable to fetch remote file - {fileLocation}");
                }

                return await response.Content.ReadAsStringAsync();
            }

            return File.ReadAllText(fileLocation);
        }

    }
}
