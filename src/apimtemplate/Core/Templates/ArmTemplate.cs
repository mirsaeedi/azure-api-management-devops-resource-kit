using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.Templates
{
	public class ArmTemplate
	{
		private List<ArmTemplateResource> _resources = new List<ArmTemplateResource>();

		[JsonProperty(PropertyName = "$schema")]
		public string Schema => GlobalConstants.TemplateSchema;

		public string ContentVersion => GlobalConstants.TemplateContentVesion;

		public Dictionary<string, ArmTemplateParameter> Parameters { get; set; } = new Dictionary<string, ArmTemplateParameter>();

		public Dictionary<string, ArmTemplateVariable> Variables { get; set; }

		public IReadOnlyList<ArmTemplateResource> Resources => _resources.AsReadOnly();

		public void AddResources(IEnumerable<ArmTemplateResource> resources)
		{
			foreach (var resource in resources)
			{
				AddResource(resource);
			}
		}

		public void AddResource(ArmTemplateResource resource)
		{
			_resources.Add(resource);
		}

		internal void AddParameter(string parameterName, ArmTemplateParameter apimServiceNameProperties)
		{
			Parameters.Add(parameterName, apimServiceNameProperties);
		}
	}
}