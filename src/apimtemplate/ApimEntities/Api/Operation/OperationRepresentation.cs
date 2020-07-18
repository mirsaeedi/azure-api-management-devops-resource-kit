namespace Apim.DevOps.Toolkit.ApimEntities.Api.Operation
{
	public class OperationRepresentation
	{
		public string ContentType { get; set; }

		public string Sample { get; set; }

		public string SchemaId { get; set; }

		public string TypeName { get; set; }

		public OperationParameter[] FormParameters { get; set; }
	}
}
