using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates
{
	/// <summary>
	/// This class manifests the final arm template. It contains all requires variables, parametes, and resources.
	/// </summary>
	public class ArmTemplate
	{
		private Dictionary<string, ArmTemplateVariable> _variables = new Dictionary<string, ArmTemplateVariable>();

		private Dictionary<string, ArmTemplateParameter> _parameters = new Dictionary<string, ArmTemplateParameter>();

		private List<ArmTemplateResource> _resources = new List<ArmTemplateResource>();

		[JsonProperty(PropertyName = "$schema")]
		public string Schema => GlobalConstants.TemplateSchema;

		public string ContentVersion => GlobalConstants.TemplateContentVesion;

		public IReadOnlyDictionary<string, ArmTemplateParameter> Parameters => _parameters;

		public IReadOnlyDictionary<string, ArmTemplateVariable> Variables => _variables;

		public IReadOnlyList<ArmTemplateResource> Resources => _resources;

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

		internal void AddParameter(string parameterName, ArmTemplateParameter parameter)
		{
			_parameters.Add(parameterName, parameter);
		}

		internal void AddVariable(string variableName, ArmTemplateVariable variable)
		{
			_variables.Add(variableName, variable);
		}
	}
}