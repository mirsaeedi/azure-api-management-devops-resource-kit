using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Serialization;
using System.Threading.Tasks;

namespace Apim.DevOps.Toolkit.Core.Infrastructure
{
	public class FileWriter
	{
		public async Task WriteJsonAsync<T>(T template, string filePath)
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

			await File.WriteAllTextAsync(filePath, jsonString);
		}
	}
}
