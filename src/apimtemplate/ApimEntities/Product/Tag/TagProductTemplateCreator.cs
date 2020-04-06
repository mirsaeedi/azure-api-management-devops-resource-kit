using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class TagProductTemplateCreator
    {
		private readonly IEnumerable<TagDeploymentDefinition> _tags;

		public TagProductTemplateCreator(IEnumerable<TagDeploymentDefinition> tags)
		{
			_tags = tags ?? new TagDeploymentDefinition[0];
		}

        public List<TagProductTemplateResource> Create(ProductDeploymentDefinition product, string[] dependsOn)
        {
            var tagProductTemplates = new List<TagProductTemplateResource>();

			var tagDisplayNames = product.TagList;

            foreach(string tagDisplayName in tagDisplayNames)
            {
				var tagName = GetTagName(tagDisplayName);

				var tagProductTemplate = new TagProductTemplateResource()
				{
					Name = $"[concat(parameters('ApimServiceName'), '/{product.Name}/{tagName}')]",
					Properties = new TagProductTemplateProperties(),
					DependsOn = dependsOn
				};

				tagProductTemplates.Add(tagProductTemplate);
            }
            return tagProductTemplates;
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
