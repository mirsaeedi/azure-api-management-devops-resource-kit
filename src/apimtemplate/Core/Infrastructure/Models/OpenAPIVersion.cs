using Newtonsoft.Json;

namespace Apim.DevOps.Toolkit.Core.Infrastructure.Models
{
	/// <summary>
	/// https://github.com/OAI/OpenAPI-Specification/blob/master/versions/3.0.0.md#fixed-fields
	/// </summary>
	public class OpenAPIVersion
	{
		// OASv2 has the property 'swagger'
		[JsonProperty(PropertyName = "swagger")]
		public string Swagger { get; set; }
		// OASv3 has the property 'openapi'
		[JsonProperty(PropertyName = "openapi")]
		public string OpenApi { get; set; }
	}
}