using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class TagApiTemplateCreator
    {
		private readonly IEnumerable<TagDeploymentDefinition> _tags;

		public TagApiTemplateCreator(IEnumerable<TagDeploymentDefinition> tags)
		{
			_tags = tags ?? new TagDeploymentDefinition[0];
		}

        public List<TagApiTemplateResource> CreateTagApiTemplateResources(ApiDeploymentDefinition api, string[] dependsOn)
        {
            var tagApiTemplates = new List<TagApiTemplateResource>();

			var tagDisplayNames = api.TagList;

            foreach(string tagDisplayName in tagDisplayNames)
            {
				var tagName = GetTagName(tagDisplayName);

				var tagApiTemplate = new TagApiTemplateResource()
				{
					Name = $"[concat(parameters('ApimServiceName'), '/{api.Name}/{tagName}')]",
					Properties = new TagApiTemplateProperties(),
					DependsOn = dependsOn
				}; 

                tagApiTemplates.Add(tagApiTemplate);
            }
            return tagApiTemplates;
        }

		private string GetTagName(string name)
		{
			var tagName = _tags.SingleOrDefault(q => q.DisplayName == name)?.Name;

			if (tagName == null)
			{
				tagName = _tags.SingleOrDefault(q => q.Name == name)?.Name;
			}

			if (tagName == null)
				return name;

			return tagName;
		}
	}
}
