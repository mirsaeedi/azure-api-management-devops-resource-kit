namespace Apim.DevOps.Toolkit.Core.Templates
{
	public class TemplateParameter
	{
		public string Type { get; set; }
		public TemplateParameterMetadata Metadata { get; set; }
		public string[] AllowedValues { get; set; }
		public string DefaultValue { get; set; }
		public string Value { get; set; }
	}
}