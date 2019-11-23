using Apim.DevOps.Toolkit.ArmTemplates;
using Apim.DevOps.Toolkit.Extensions;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
	public class ProductDeploymentDefinition : ProductsProperties
	{
		/// <summary>
		/// The Id of the product
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Local path or url to policy
		/// </summary>
		public string Policy { get; set; }

		public string Tags { get; set; }

		public bool IsDependOnTags() => Tags != null;

		public IEnumerable<string> TagList => Tags.GetItems(new string[0]);

		internal bool IsDependOnPolicy() => Policy != null;
	}

}