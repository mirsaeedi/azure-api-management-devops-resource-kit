using Newtonsoft.Json;
using System.IO;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class FileWriter
    {
        public void WriteJson(object template, string location)
        {
            // writes json object to provided location
            string jsonString = JsonConvert.SerializeObject(template,
                            Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                ContractResolver = new DefaultContractResolver
                                {
                                    NamingStrategy = new CamelCaseNamingStrategy()
                                },
                                NullValueHandling = NullValueHandling.Ignore

                            });

            File.WriteAllText(location, jsonString);
        }

        public void WriteXMLToFile(string xmlContent, string location)
        {
            // writes xml content to provided location
            File.WriteAllText(location, xmlContent);
        }

        public void CreateFolderIfNotExists(string folderLocation)
        {
            // creates directory if it does not already exist
            System.IO.Directory.CreateDirectory(folderLocation);
        }
    }
}
