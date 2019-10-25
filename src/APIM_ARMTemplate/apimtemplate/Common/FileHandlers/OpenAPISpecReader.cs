using Apim.DevOps.Toolkit.Extensions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common
{
    public class OpenAPISpecReader
    {
        private string _content = null;
        private string _openApiFilePath;
        public OpenAPISpecReader(string openApiFilePath)
        {
            _openApiFilePath = openApiFilePath;
        }
        public async Task<string> GetOpenApiVersion()
        {
            var contents = await GetContents();

            OpenAPISpecWithVersion openAPISpecWithVersion = JsonConvert.DeserializeObject<OpenAPISpecWithVersion>(contents);
            return openAPISpecWithVersion.Swagger != null ? openAPISpecWithVersion.Swagger : openAPISpecWithVersion.OpenApi;
        }

        private async Task<string> GetContents()
        {
            if (_content != null)
                return _content;

            var fileReader = new FileReader();
            _content = await fileReader.RetrieveFileContentsAsync(_openApiFilePath);
            return _content;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/rest/api/apimanagement/2019-01-01/apis/createorupdate#contentformat
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetOpenApiFormat()
        {
            var contents = await GetContents();
            var version = await GetOpenApiVersion();
            var isUrl = _openApiFilePath.IsUri(out _);

            if (isUrl)
            {
                if (contents.IsJson() && version.StartsWith("2"))
                    return "swagger-link-json";
                else if (contents.IsJson() && version.StartsWith("3"))
                    return "openapi-link";

                throw new Exception("Unsupported openapi format");
            }

            if (contents.IsJson() && version.StartsWith("2"))
                return "swagger-json";
            else if (contents.IsJson() && version.StartsWith("3"))
                return "openapi+json";

            return "openapi";
        }

        internal async Task<string> GetValue()
        {
            return _openApiFilePath.IsUri(out _) ? _openApiFilePath : await GetContents();
        }
    }

    /// <summary>
    /// https://github.com/OAI/OpenAPI-Specification/blob/master/versions/3.0.0.md#fixed-fields
    /// </summary>
    public class OpenAPISpecWithVersion
    {
        // OASv2 has the property 'swagger'
        [JsonProperty(PropertyName = "swagger")]
        public string Swagger { get; set; }
        // OASv3 has the property 'openapi'
        [JsonProperty(PropertyName = "openapi")]
        public string OpenApi { get; set; }
    }

}