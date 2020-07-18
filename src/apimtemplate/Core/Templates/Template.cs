using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.Templates
{
	public class Template
	{
		private List<TemplateResource> _resources = new List<TemplateResource>();

		[JsonProperty(PropertyName = "$schema")]
		public string Schema => GlobalConstants.TemplateSchema;

		public string ContentVersion => GlobalConstants.ApiVersion;

		public Dictionary<string, TemplateParameter> Parameters { get; set; } = new Dictionary<string, TemplateParameter>();

		public Dictionary<string, TemplateVariable> Variables { get; set; }

		public IReadOnlyList<TemplateResource> Resources => _resources.AsReadOnly();

		public void AddResources(IEnumerable<TemplateResource> resources)
		{
			foreach (var resource in resources)
			{
				AddResource(resource);
			}
		}

		public void AddResource(TemplateResource resource)
		{
			_resources.Add(resource);
		}

		internal void AddParameter(string parameterName, TemplateParameter apimServiceNameProperties)
		{
			Parameters.Add(parameterName, apimServiceNameProperties);
		}
	}
}