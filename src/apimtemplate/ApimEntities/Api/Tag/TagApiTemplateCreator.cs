using System.Collections.Generic;
using System.Linq;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class TagApiTemplateCreator
    {
		private readonly IEnumerable<TagDeploymentDefinition> _tags;

		public TagApiTemplateCreator(IEnumerable<TagDeploymentDefinition> tags)
		{
			_tags = tags;
		}

        public List<TagApiTemplateResource> CreateTagApiTemplateResources(ApiDeploymentDefinition api, string[] dependsOn)
        {
            var tagApiTemplates = new List<TagApiTemplateResource>();

			var tagDisplayNames = api.TagList;

            foreach(string tagDisplayName in tagDisplayNames)
            {
				var tag = _tags.Single(q => q.DisplayName == tagDisplayName);

				var tagApiTemplate = new TagApiTemplateResource()
				{
					Name = $"[concat(parameters('ApimServiceName'), '/{api.Name}/{tag.Name}')]",
					Properties = new TagApiTemplateProperties(),
					DependsOn = dependsOn
				}; 

                tagApiTemplates.Add(tagApiTemplate);
            }
            return tagApiTemplates;
        }
    }
}
