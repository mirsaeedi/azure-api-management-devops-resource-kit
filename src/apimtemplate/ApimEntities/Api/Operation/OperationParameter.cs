namespace Apim.DevOps.Toolkit.ApimEntities.Api.Operation
{

	public class OperationParameter
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string Type { get; set; }

		public string DefaultValue { get; set; }

		public bool Required { get; set; }

		public string[] Values { get; set; }
	}
}
