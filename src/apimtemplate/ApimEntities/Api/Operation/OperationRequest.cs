namespace Apim.DevOps.Toolkit.ApimEntities.Api.Operation
{
	public class OperationRequest
	{
		public string Description { get; set; }
		public OperationParameter[] QueryParameters { get; set; }
		public OperationParameter[] Headers { get; set; }
		public OperationRepresentation[] Representations { get; set; }
	}
}
