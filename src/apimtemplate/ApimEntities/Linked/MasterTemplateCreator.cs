using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.DevOps.Toolkit.Core.Configuration;
using Apim.DevOps.Toolkit.Core.Templates;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
	public class MasterTemplateCreator
	{
		private Template EmptyApimServiceTemplate => new Template
		{
			Parameters = new Dictionary<string, TemplateParameter>
			{
				{
					"ApimServiceName", new TemplateParameter
					{
						Type = "string",
					}
				}
			},
		};

		private Template EmptyTemplate => new Template
		{
			Parameters = new Dictionary<string, TemplateParameter>()
		};

		public async Task<Template> Create(IEnumerable<TemplateResource> resources)
		{
			var template = EmptyApimServiceTemplate;
			template.Parameters = this.CreateMasterTemplateParameters();
			template.AddResources(resources);

			return template;
		}

		public Template CreateMasterTemplateParameterValues(DeploymentDefinition deploymentDefinition)
		{
			var template = EmptyTemplate;

			TemplateParameter apimServiceNameProperties = new TemplateParameter()
			{
				Value = deploymentDefinition.ApimServiceName
			};

			template.AddParameter("ApimServiceName", apimServiceNameProperties);

			return template;
		}

		private Dictionary<string, TemplateParameter> CreateMasterTemplateParameters()
		{
			Dictionary<string, TemplateParameter> parameters = new Dictionary<string, TemplateParameter>();

			TemplateParameter apimServiceNameProperties = new TemplateParameter()
			{
				Metadata = new TemplateParameterMetadata()
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
