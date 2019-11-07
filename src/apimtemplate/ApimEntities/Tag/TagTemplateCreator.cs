using Apim.Arm.Creator.Creator.TemplateCreators;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Apim.DevOps.Toolkit.ApimEntities.Tag
{
	public class TagTemplateCreator : TemplateCreator, ITemplateCreator
	{
		public async Task<Template> Create(DeploymentDefinition creatorConfig)
		{
			var template = EmptyTemplate;
			var resources = new List<TemplateResource>();

			foreach (var tag in creatorConfig.Tags)
			{
				var tagTemplateResource = new TagTemplateResouce()
				{
					Name = $"[concat(parameters('ApimServiceName'), '/{tag.Name}')]",
					Properties = new TagPropertise()
					{
						DisplayName = tag.DisplayName
					},
					DependsOn = new string[0]
				};

				resources.Add(tagTemplateResource);
			}

			template.Resources = resources.ToArray();

			return await Task.FromResult(template);
		}
	}
}
