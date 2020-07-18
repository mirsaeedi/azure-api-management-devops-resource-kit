namespace Apim.DevOps.Toolkit.ApimEntities.Api.Operation
{

	public class OperationsResponse
	{
		public int StatusCode { get; set; }

		public string Description { get; set; }

		public OperationParameter[] Headers { get; set; }

		public OperationRepresentation[] Representations { get; set; }
	}
}
