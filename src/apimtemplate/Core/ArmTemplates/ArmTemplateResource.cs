using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates
{
	public abstract class ArmTemplateResource
	{
		private List<string> _dependencies = new List<string>();

		public ArmTemplateResource(string identifier, string name, string type, IEnumerable<string> dependencies)
		{
			Identifier = identifier.Split('/').Select(id => $"'{id}'").Aggregate((a,b) => $"{a}, {b}" );
			Name = name;
			Type = type;
			_dependencies.AddRange(dependencies);
		}
		private string Identifier { get; set; }

		public string Name { get; set; }

		public string Type { get; set; }

		public string ApiVersion { get; set; } = GlobalConstants.ApiVersion;

		public string Scale { get; set; }

		public IReadOnlyList<string> DependsOn => _dependencies.AsReadOnly();

		public IReadOnlyList<ArmTemplateResource> Resources { get; set; }

		public string ResourceId() => $"[resourceId('{Type}', parameters('ApimServiceName'), {Identifier})]";

		public void AddDependencies(IEnumerable<ArmTemplateResource> dependencies)
		{
			AddDependencies(dependencies.Select(dependency => dependency.ResourceId()));
		}

		public void AddDependencies(IEnumerable<string> dependencies)
		{
			foreach (var dependency in dependencies)
			{
				if (!dependencies.Contains(dependency))
				{
					_dependencies.Add(dependency);
				}
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