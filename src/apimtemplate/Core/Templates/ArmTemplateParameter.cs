namespace Apim.DevOps.Toolkit.Core.Templates
{
	public class ArmTemplateParameter
	{
		public string Type { get; set; }
		public ArmTemplateParameterMetadata Metadata { get; set; }
		public string[] AllowedValues { get; set; }
		public string DefaultValue { get; set; }
		public string Value { get; set; }
	}
}