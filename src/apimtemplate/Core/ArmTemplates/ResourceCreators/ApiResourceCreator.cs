﻿using Apim.DevOps.Toolkit.ApimEntities.Api;
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
using Newtonsoft.Json;
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

			Console.WriteLine("Creating api resources");
			Console.WriteLine("------------------------------------------");

			var resources = new List<ArmTemplateResource>();

			resources.AddRange(CreateApis(deploymentDefinition));
			resources.AddRange(CreateUpdateDisplayName(deploymentDefinition));
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
											Value = isUrl ? operationPolicy : fileReader.RetrieveFileContentsAsync(operationPolicy).Result
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
							.CreateResourcesIf(d => d.HasPolicy(), true);
		}

		private IEnumerable<ArmTemplateResource<ApiProperties>> CreateApis(DeploymentDefinition deploymentDefinition)
		{
			return new ArmTemplateResourceCreator<ApiDeploymentDefinition, ApiProperties>(_mapper)
							.ForDeploymentDefinitions(deploymentDefinition.Apis)
							.WithName(d => d.Name)
							.OfType(ResourceType.Api)
							.CheckDependencies()
							.CreateResources(true);
		}

		private IEnumerable<ArmTemplateResource<ApiUpdateDisplayNameProperties>> CreateUpdateDisplayName(DeploymentDefinition deploymentDefinition)
		{
			return new ArmTemplateResourceCreator<ApiDeploymentDefinition, ApiUpdateDisplayNameProperties>(_mapper)
							.ForDeploymentDefinitions(deploymentDefinition.Apis)
							.UseResourceCreator(apiDeploymentDefinition =>
							{
								var templateResources = new List<ArmTemplateResource<ApiUpdateDisplayNameProperties>>();
								var templateResource = new ArmTemplateResource<ApiUpdateDisplayNameProperties>(
									$"{apiDeploymentDefinition.Name}-updateDisplayName",
									$"{apiDeploymentDefinition.Name}-updateDisplayName",
									ResourceType.Deployment,
									new ApiUpdateDisplayNameProperties
									{
										Template = CreateUpdateDisplayNameNestedTemplate(apiDeploymentDefinition)
									},
									new string[] { $"[resourceId('{ResourceType.Api}', parameters('ApimServiceName'), '{apiDeploymentDefinition.Name}')]" });

								templateResource.ApiVersion = GlobalConstants.DeploymentApiVersion;
								templateResources.Add(templateResource);

								return templateResources;
							})
							.CreateResources(true);
		}

		private ArmTemplate CreateUpdateDisplayNameNestedTemplate(ApiDeploymentDefinition apiDeploymentDefinition)
        {
			var nestedTemplate = new ArmTemplate();
			var updateApiResource = new ArmTemplateResource<ApiProperties>(
					$"{apiDeploymentDefinition.Name}/api",
					$"[concat(parameters('ApimServiceName'), '/{apiDeploymentDefinition.Name}')]",
					ResourceType.Api,
					new ApiProperties
                    {
						Path = $"[reference(resourceId('Microsoft.ApiManagement/service/apis', parameters('ApimServiceName'), '{apiDeploymentDefinition.Name}')).path]",
						DisplayName = apiDeploymentDefinition.DisplayName,
						Description = $"[reference(resourceId('Microsoft.ApiManagement/service/apis', parameters('ApimServiceName'), '{apiDeploymentDefinition.Name}')).description]",
						ServiceUrl = $"[reference(resourceId('Microsoft.ApiManagement/service/apis', parameters('ApimServiceName'), '{apiDeploymentDefinition.Name}')).serviceUrl]",
						Protocols = apiDeploymentDefinition.Protocols.GetItems(new[] { "https" })
                    }, Array.Empty<string>());
			nestedTemplate.AddResource(updateApiResource);
			return nestedTemplate;
        }
	}
}
