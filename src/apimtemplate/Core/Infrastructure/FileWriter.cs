using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
	public class FileWriter
	{
		public void WriteJson<T>(T template, string filePath)
		{
			var jsonString = JsonConvert.SerializeObject(template,
							Formatting.Indented,
							new JsonSerializerSettings
							{
								ContractResolver = new DefaultContractResolver
								{
									NamingStrategy = new CamelCaseNamingStrategy()
								},
								NullValueHandling = NullValueHandling.Ignore,
								Converters = { new Newtonsoft.Json.Converters.StringEnumConverter(new CamelCaseNamingStrategy()) }
							});

			var fileInfo = new FileInfo(filePath);

			if (!fileInfo.Directory.Exists)
				fileInfo.Directory.Create();

			File.WriteAllText(filePath, jsonString);
		}
	}
}
