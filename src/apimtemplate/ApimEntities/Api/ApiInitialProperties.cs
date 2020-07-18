namespace Apim.DevOps.Toolkit.ApimEntities.Api
{
	public class ApiInitialProperties
	{
		public ApiInitialProperties(string path, string format, string value)
		{
			Path = path;
			Format = format;
			Value = value;
		}

		public string Path { get; set; }
		public string Value { get; set; }
		public string Format { get; set; }
	}
}
