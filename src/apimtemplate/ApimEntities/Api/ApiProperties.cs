namespace Apim.DevOps.Toolkit.ApimEntities.Api
{
	public class ApiProperties
	{
		public string Value { get; set; }

		public string Format { get; set; }

		public string Description { get; set; }

		public ApiTemplateAuthenticationSettings AuthenticationSettings { get; set; }

		public ApiTemplateSubscriptionKeyParameterNames SubscriptionKeyParameterNames { get; set; }

		public string Type { get; set; }

		public string ApiRevision { get; set; }

		public string ApiVersion { get; set; }

		public bool? IsCurrent { get; set; }

		public string ApiRevisionDescription { get; set; }

		public string ApiVersionDescription { get; set; }

		public string ApiVersionSetId { get; set; }

		public bool? SubscriptionRequired { get; set; }

		public string SourceApiId { get; set; }

		public string DisplayName { get; set; }

		public string ServiceUrl { get; set; }

		public string Path { get; set; }

		public string[] Protocols { get; set; }

		public ApiTemplateWSDLSelector WsdlSelector { get; set; }

		public string ApiType { get; set; }
	}
}
