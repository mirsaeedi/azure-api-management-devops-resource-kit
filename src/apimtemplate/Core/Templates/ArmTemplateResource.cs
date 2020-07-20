using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.Templates
{
	public abstract class ArmTemplateResource
	{
		private List<string> _dependencies = new List<string>();

		public ArmTemplateResource(string identifier, string name, string type, IEnumerable<string> dependencies)
		{
			Identifier = identifier;
			Name = name;
			Type = type;
			_dependencies.AddRange(dependencies);
		}
		private string Identifier { get; set; }

		private string ResourceId => $"[resourceId('{Type}', parameters('ApimServiceName'), '{Identifier}')]";

		public string Name { get; set; }

		public string Type { get; set; }

		public string ApiVersion { get; set; } = GlobalConstants.ApiVersion;

		public string Scale { get; set; }

		public IReadOnlyList<string> DependsOn => _dependencies.AsReadOnly();

		public IReadOnlyList<ArmTemplateResource> Resources { get; set; }

		internal void AddDependencies(IEnumerable<ArmTemplateResource> dependencies)
		{
			foreach (var dependency in dependencies)
			{
				_dependencies.Add(dependency.ResourceId);
			}
		}
	}

	public class ArmTemplateResource<TProperties> : ArmTemplateResource
	{
		public ArmTemplateResource(string identifier, string name, string type, TProperties properties, IEnumerable<string> dependencies)
			: base(identifier, name, type, dependencies)
		{
			Properties = properties;
		}

		public TProperties Properties { get; set; }
	}
}