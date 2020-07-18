namespace Apim.DevOps.Toolkit.Core.Infrastructure.Constants
{
	public static class OpenApiContentFormat
	{
		/// <summary>
		/// The Open Api 2.0 document is hosted on a publicly accessible Internet address.
		/// </summary>
		public static readonly string SwaggerLinkJson = "swagger-link-json";

		/// <summary>
		/// The contents are inline and Content Type is a OpenApi 2.0 Document.
		/// </summary>
		public static readonly string SwaggerJson = "swagger-json";

		/// <summary>
		/// The Open Api 3.0 Json document is hosted on a publicly accessible Internet address.
		/// </summary>
		public static readonly string OpenApiLink = "openapi-link";

		/// <summary>
		/// The contents are inline and Content Type is a OpenApi 3.0 Document in JSON format.
		/// </summary>
		public static readonly string OpenApiJson = "openapi+json";

		/// <summary>
		/// The contents are inline and Content Type is a OpenApi 3.0 Document in YAML format.
		/// </summary>
		public static readonly string OpenApi = "openapi";
	}
}
