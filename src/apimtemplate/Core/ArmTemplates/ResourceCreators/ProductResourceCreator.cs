using Apim.DevOps.Toolkit.ApimEntities.Product;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates.ResourceCreators
{
	public class ProductResourceCreator : IResourceCreator
	{
		private IMapper _mapper;

		public ProductResourceCreator(IMapper mapper)
		{
			_mapper = mapper;
		}
		public IEnumerable<ArmTemplateResource> Create(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.Products.Count() == 0)
			{
				return Array.Empty<ArmTemplateResource>();
			}

			Console.WriteLine("Creating products template");
			Console.WriteLine("------------------------------------------");

			var resources = new List<ArmTemplateResource>();

			resources.AddRange(CreateProducts(deploymentDefinition));

			resources.AddRange(CreateProductPolicies(deploymentDefinition));

			resources.AddRange(CreateProductTags(deploymentDefinition));

			return resources;
		}

		private IEnumerable<ArmTemplateResource<TagProductProperties>> CreateProductTags(DeploymentDefinition deploymentDefinition)
		{
			return new ArmTemplateResourceCreator<ProductDeploymentDefinition, TagProductProperties>(_mapper)
							.ForDeploymentDefinitions(deploymentDefinition.Products)
							.UseResourceCreator(ProductDeploymentDefinition =>
							{
								var templateResources = new List<ArmTemplateResource<TagProductProperties>>();

								foreach (string tagDisplayName in ProductDeploymentDefinition.TagList)
								{
									var tagName = ProductDeploymentDefinition.GetTagName(tagDisplayName);
									var templateRsource = new ArmTemplateResource<TagProductProperties>(
										$"{ProductDeploymentDefinition.Name}/{tagName}",
										$"[concat(parameters('ApimServiceName'), '/{ProductDeploymentDefinition.Name}/{tagName}')]",
										ResourceType.TagProduct,
										new TagProductProperties(),
										new string[]
										{
								$"[resourceId('{ResourceType.Product}', parameters('ApimServiceName'), '{ProductDeploymentDefinition.Name}')]",
								$"[resourceId('{ResourceType.Tag}', parameters('ApimServiceName'), '{tagName}')]"
										});

									templateResources.Add(templateRsource);
								}

								return templateResources;
							})
							.CreateResourcesIf(d => d.Tags != null, true);
		}

		private IEnumerable<ArmTemplateResource<ProductPolicyProperties>> CreateProductPolicies(DeploymentDefinition deploymentDefinition)
		{
			return new ArmTemplateResourceCreator<ProductDeploymentDefinition, ProductPolicyProperties>(_mapper)
							.ForDeploymentDefinitions(deploymentDefinition.Products)
							.WithName(d => $"{d.Name}/policy")
							.OfType(ResourceType.ProductPolicy)
							.WhichDependsOnResourceOfType(ResourceType.Product)
							.WhichDependsOnResourceWithName(d => d.Name)
							.CreateResourcesIf(d => d.Policy != null, true);
		}

		private IEnumerable<ArmTemplateResource<ProductsProperties>> CreateProducts(DeploymentDefinition deploymentDefinition)
		{
			return new ArmTemplateResourceCreator<ProductDeploymentDefinition, ProductsProperties>(_mapper)
							.ForDeploymentDefinitions(deploymentDefinition.Products)
							.WithName(d => d.Name)
							.OfType(ResourceType.Product)
							.CheckDependencies()
							.CreateResources();
		}
	}
}
