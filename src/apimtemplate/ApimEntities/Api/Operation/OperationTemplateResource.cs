namespace Apim.DevOps.Toolkit.ApimEntities.Api.Operation
{
	public class OperationProperties
	{
		public OperationParameter[] TemplateParameters { get; set; }

		public string Description { get; set; }

		public OperationRequest Request { get; set; }

		public OperationsResponse[] Responses { get; set; }

		public string Policies { get; set; }

		public string DisplayName { get; set; }

		public string Method { get; set; }

		public string UrlTemplate { get; set; }
	}
}
