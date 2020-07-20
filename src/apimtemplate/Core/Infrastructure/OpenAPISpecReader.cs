using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using Apim.DevOps.Toolkit.Core.Infrastructure.Models;
using Apim.DevOps.Toolkit.Extensions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Apim.DevOps.Toolkit.Core.Infrastructure
{
	public class OpenApiSpecReader
	{
		private readonly FileReader _fileReader = new FileReader();
		private string _openApiFilePath;
		public OpenApiSpecReader(string openApiFilePath)
		{
			_openApiFilePath = openApiFilePath;
		}

		/// <summary>
		/// https://docs.microsoft.com/en-us/rest/api/apimanagement/2019-01-01/apis/createorupdate#contentformat
		/// </summary>
		/// <returns></returns>
		public async Task<string> GetOpenApiFormat()
		{
			var content = await GetContent();
			var isJson = content.IsJson();
			var isYaml = content.IsYaml();

			if (!isJson || isYaml)
			{
				throw new Exception("Unsupported OpenApi format. The OpenApi document should be provided in json format. Version 2 and 3 of OpenApi are supported");
			}

			var version = this.GetOpenApiVersion(content);

			var isUrl = _openApiFilePath.IsUri(out _);

			if (isUrl)
			{
				if (isJson && version.Major == 2)
					return OpenApiContentFormat.SwaggerLinkJson;
				else if (isJson && version.Major == 3)
					return OpenApiContentFormat.OpenApiLink;
			}

			if (isJson && version.Major == 2)
				return OpenApiContentFormat.SwaggerJson;
			else if (isJson && version.Major == 3)
				return OpenApiContentFormat.OpenApiJson;
			else if (isYaml && version.Major == 3)
				return OpenApiContentFormat.OpenApi;

			throw new Exception("Unsupported OpenApi format. The OpenApi document should be provided in json format. Version 2 and 3 of OpenApi are supported");
		}

		public async Task<string> GetValue(string apiTitle)
		{
			var swaggerJson = await GetContent();

			var swagger = JsonConvert.DeserializeObject<dynamic>(swaggerJson);
			swagger.info.title = apiTitle;
			return JsonConvert.SerializeObject(swagger);
		}

		private async Task<string> GetContent()
		{
			return await _fileReader.RetrieveFileContentsAsync(_openApiFilePath);
		}

		private SemanticVersion GetOpenApiVersion(string openApiDocument)
		{
			var openAPISpecWithVersion = JsonConvert.DeserializeObject<OpenAPIVersion>(openApiDocument);
			return SemanticVersion.FromString(openAPISpecWithVersion.Swagger != null ? openAPISpecWithVersion.Swagger : openAPISpecWithVersion.OpenApi);
		}
	}
}