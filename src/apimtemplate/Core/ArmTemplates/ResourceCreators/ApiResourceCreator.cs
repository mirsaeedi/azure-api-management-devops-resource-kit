using Apim.DevOps.Toolkit.ApimEntities.Api;
using Apim.DevOps.Toolkit.ApimEntities.Api.Operation.Policy;
using Apim.DevOps.Toolkit.ApimEntities.Api.Policy;
using Apim.DevOps.Toolkit.ApimEntities.Api.Product;
using Apim.DevOps.Toolkit.ApimEntities.Api.Tag;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using Apim.DevOps.Toolkit.Extensions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates.ResourceCreators
{
	public class ApiResourceCreator : IResourceCreator
	{
		private IMapper _mapper;

		public ApiResourceCreator(IMapper mapper)
		{
			_mapper = mapper;
		}
		public IEnumerable<ArmTemplateResource> Create(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.Apis.Count() == 0)
			{
				return Array.Empty<ArmTemplateResource>();
			}

			Console.WriteLine("Creating api subsequent template");
			Console.WriteLine("------------------------------------------");

			var resources = new List<ArmTemplateResource>();

			resources.AddRange(CreateApis(deploymentDefinition));
			resources.AddRange(CreateApiPolicies(deploymentDefinition));
			resources.AddRange(CreateOperationPolicies(deploymentDefinition));
			resources.AddRange(CreateProductApis(deploymentDefinition));
			resources.AddRange(CreateProductTags(deploymentDefinition));

			return resources;
		}

		private IEnumerable<ArmTemplateResource<TagApiTemplateProperties>> CreateProductTags(DeploymentDefinition deploymentDefinition)
		{
			return new ArmTemplateResourceCreator<ApiDeploymentDefinition, TagApiTemplateProperties>(_mapper)
							.ForDeploymentDefinitions(deploymentDefinition.Apis)
							.UseResourceCreator(apiDeploymentDefinition =>
							{
								var templateResources = new List<ArmTemplateResource<TagApiTemplateProperties>>();

								foreach (string tagDisplayName in apiDeploymentDefinition.TagList)
								{
									var tagName = apiDeploymentDefinition.GetTagName(tagDisplayName);

									var templateResource = new ArmTemplateResource<TagApiTemplateProperties>(
										$"{apiDeploymentDefinition.Name}/{tagName}",
										$"[concat(parameters('ApimServiceName'), '/{apiDeploymentDefinition.Name}/{tagName}')]",
										ResourceType.TagApi,
										new TagApiTemplateProperties(),
										new string[]
										{
								$"[resourceId('{ResourceType.Api}', parameters('ApimServiceName'), '{apiDeploymentDefinition.Name}')]",
								$"[resourceId('{ResourceType.Tag}', parameters('ApimServiceName'), '{tagName}')]"
										});

									templateResources.Add(templateResource);
								}

								return templateResources;
							})
							.CreateResourcesIf(d => d.IsDependentOnTags(), true);
		}

		private IEnumerable<ArmTemplateResource<ProductApiProperties>> CreateProductApis(DeploymentDefinition deploymentDefinition)
		{
			return new ArmTemplateResourceCreator<ApiDeploymentDefinition, ProductApiProperties>(_mapper)
							.ForDeploymentDefinitions(deploymentDefinition.Apis)
							.UseResourceCreator(apiDeploymentDefinition =>
							{
								var templateResources = new List<ArmTemplateResource<ProductApiProperties>>();
								foreach (string productDisplayName in apiDeploymentDefinition.ProductList)
								{
									var productName = apiDeploymentDefinition.GetProductName(productDisplayName);
									var dependencies = new List<string>()
									{
								$"[resourceId('{ResourceType.Api}', parameters('ApimServiceName'), '{apiDeploymentDefinition.Name}')]",
								$"[resourceId('{ResourceType.Product}', parameters('ApimServiceName'), '{productName}')]"
									};

									var templateResource = new ArmTemplateResource<ProductApiProperties>(
										$"{productName}/{apiDeploymentDefinition.Name}",
										$"[concat(parameters('ApimServiceName'), '/{productName}/{apiDeploymentDefinition.Name}')]",
										ResourceType.ProductApi,
										new ProductApiProperties(),
										dependencies);

									templateResources.Add(templateResource);
								}

								return templateResources;
							})
							.CreateResourcesIf(d => d.IsDependentOnProducts(), true);
		}

		private IEnumerable<ArmTemplateResource<ApiOperationPolicyProperties>> CreateOperationPolicies(DeploymentDefinition deploymentDefinition)
		{
			return new ArmTemplateResourceCreator<ApiDeploymentDefinition, ApiOperationPolicyProperties>(_mapper)
							.ForDeploymentDefinitions(deploymentDefinition.Apis)
							.UseResourceCreator(apiDeploymentDefinition =>
							{
								var templateResources = new List<ArmTemplateResource<ApiOperationPolicyProperties>>();
								var fileReader = new FileReader();

								foreach (var pair in apiDeploymentDefinition.Operations)
								{
									var operationPolicy = pair.Value.Policy;
									var operationName = pair.Key;

									var isUrl = operationPolicy.IsUri(out _);

									var templateResource = new ArmTemplateResource<ApiOperationPolicyProperties>(
										$"{apiDeploymentDefinition.Name}/{operationName}/policy",
										$"[concat(parameters('ApimServiceName'), '/{apiDeploymentDefinition.Name}/{operationName}/policy')]",
										ResourceType.ApiOperationPolicy,
										new ApiOperationPolicyProperties()
										{
											Format = isUrl ? "rawxml-link" : "rawxml",
											Value = isUrl ? apiDeploymentDefinition.Policy : fileReader.RetrieveFileContentsAsync(apiDeploymentDefinition.Policy).Result
										},
										new string[] { $"[resourceId('{ResourceType.Api}', parameters('ApimServiceName'), '{apiDeploymentDefinition.Name}')]" });

									templateResources.Add(templateResource);
								}

								return templateResources;
							})
							.CreateResourcesIf(d => d.Operations != null, true);
		}

		private IEnumerable<ArmTemplateResource<ApiPolicyProperties>> CreateApiPolicies(DeploymentDefinition deploymentDefinition)
		{
			return new ArmTemplateResourceCreator<ApiDeploymentDefinition, ApiPolicyProperties>(_mapper)
							.ForDeploymentDefinitions(deploymentDefinition.Apis)
							.WithName(d => $"{d.Name}/policy")
							.OfType(ResourceType.ApiPolicy)
							.WhichDependsOnResourceOfType(ResourceType.Api)
							.WhichDependsOnResourceWithName(d => d.Name)
							.CreateResourcesIf(d => d.HasPolicy());
		}

		private IEnumerable<ArmTemplateResource<ApiProperties>> CreateApis(DeploymentDefinition deploymentDefinition)
		{
			return new ArmTemplateResourceCreator<ApiDeploymentDefinition, ApiProperties>(_mapper)
							.ForDeploymentDefinitions(deploymentDefinition.Apis)
							.WithName(d => d.Name)
							.OfType(ResourceType.Api)
							.CheckDependencies()
							.CreateResources();
		}
	}
}
