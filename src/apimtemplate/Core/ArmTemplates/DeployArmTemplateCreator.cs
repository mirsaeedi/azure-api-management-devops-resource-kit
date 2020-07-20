using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates
{
	public class DeployArmTemplateCreator
	{
		private ArmTemplate EmptyApimServiceTemplate => new ArmTemplate
		{
			Parameters = new Dictionary<string, ArmTemplateParameter>
			{
				{
					"ApimServiceName", new ArmTemplateParameter
					{
						Type = "string",
					}
				}
			},
		};

		private ArmTemplate EmptyTemplate => new ArmTemplate
		{
			Parameters = new Dictionary<string, ArmTemplateParameter>()
		};

		public async Task<ArmTemplate> Create(IEnumerable<ArmTemplateResource> resources)
		{
			var template = EmptyApimServiceTemplate;
			template.Parameters = this.CreateMasterTemplateParameters();
			template.AddResources(resources);

			return template;
		}

		public ArmTemplate CreateMasterTemplateParameterValues(DeploymentDefinition deploymentDefinition)
		{
			var template = EmptyTemplate;

			ArmTemplateParameter apimServiceNameProperties = new ArmTemplateParameter()
			{
				Value = deploymentDefinition.ApimServiceName
			};

			template.AddParameter("ApimServiceName", apimServiceNameProperties);

			return template;
		}

		private Dictionary<string, ArmTemplateParameter> CreateMasterTemplateParameters()
		{
			Dictionary<string, ArmTemplateParameter> parameters = new Dictionary<string, ArmTemplateParameter>();

			ArmTemplateParameter apimServiceNameProperties = new ArmTemplateParameter()
			{
				Metadata = new ArmTemplateParameterMetadata()
				{
					Description = "Name of the API Management"
				},
				Type = "string"
			};
			parameters.Add("ApimServiceName", apimServiceNameProperties);

			return parameters;
		}
	}
}
