using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using Apim.DevOps.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.ApimEntities
{
	public class ProductDeploymentDefinition : EntityDeploymentDefinition
	{
		/// <summary>
		/// The Id of the product
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Local path or url to policy
		/// </summary>
		public string Policy { get; set; }

		public string Description { get; set; }

		public string Terms { get; set; }

		public bool SubscriptionRequired { get; set; }

		public bool? ApprovalRequired { get; set; }

		public int? SubscriptionsLimit { get; set; }

		public string State { get; set; }

		public string DisplayName { get; set; }

		public string Tags { get; set; }

		public bool IsDependentOnTags() => Tags != null;

		public IEnumerable<string> TagList => Tags.GetItems(new string[0]);

		internal bool IsDependentOnPolicy() => Policy != null;

		public DeploymentDefinition Root { get; set; }

		public string GetTagName(string tag)
		{
			return Root.Tags.FirstOrDefault(tagDeploymentDefinition => tagDeploymentDefinition.DisplayName == tag || tagDeploymentDefinition.Name == tag)?.Name ?? tag;
		}

		public override IEnumerable<string> Dependencies()
		{
			var dependencies = new List<string>();

			if (IsDependentOnTags())
			{
				var dependentTags = TagList.Select(tag => $"[resourceId('{ResourceType.Tag}', parameters('ApimServiceName'), '{GetTagName(tag)}')]");

				dependencies.AddRange(dependentTags);
			}

			return dependencies;
		}
	}
}