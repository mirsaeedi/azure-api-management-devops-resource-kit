namespace Apim.DevOps.Toolkit.Core.Infrastructure.Models
{
	public class SemanticVersion
	{
		public int Major { get; set; }

		public int Minor { get; set; }

		public int Patch { get; set; }

		public static SemanticVersion FromString(string semanticVersion)
		{
			var parts = semanticVersion.Split('.');
			var patch = parts.Length == 3 ? int.Parse(parts[0]) : 0;
			return new SemanticVersion
			{
				Major = int.Parse(parts[0]),
				Minor = int.Parse(parts[1]),
				Patch = patch
			};
		}
	}
}
