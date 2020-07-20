using Apim.DevOps.Toolkit.ApimEntities.Api;
using Apim.DevOps.Toolkit.ApimEntities.Api.Operation.Policy;
using Apim.DevOps.Toolkit.ApimEntities.Api.Policy;
using Apim.DevOps.Toolkit.ApimEntities.Api.Product;
using Apim.DevOps.Toolkit.ApimEntities.Api.Tag;
using Apim.DevOps.Toolkit.ApimEntities.ApiVersionSet;
using Apim.DevOps.Toolkit.ApimEntities.AuthotizationServer;
using Apim.DevOps.Toolkit.ApimEntities.Backend;
using Apim.DevOps.Toolkit.ApimEntities.Logger;
using Apim.DevOps.Toolkit.ApimEntities.Policy;
using Apim.DevOps.Toolkit.ApimEntities.Product;
using Apim.DevOps.Toolkit.ApimEntities.Subscription;
using Apim.DevOps.Toolkit.ApimEntities.Tag;
using Apim.DevOps.Toolkit.ApimEntities.User;
using Apim.DevOps.Toolkit.ArmTemplates;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.ApimEntities;
using Apim.DevOps.Toolkit.Core.Infrastructure;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using Apim.DevOps.Toolkit.Extensions;
using AutoMapper;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates
{
	public class ArmTemplateCreator
	{
		private readonly DeploymentDefinition _deploymentDefinition;
		private readonly IMapper _mapper;
		private readonly string _masterFileName;
		private readonly string _fileNamePrefix;
		private readonly FileWriter _fileWriter = new FileWriter();

		public ArmTemplateCreator(DeploymentDefinition deploymentDefinition, string masterFileName, string fileNamePrefix, IMapper mapper)
		{
			_deploymentDefinition = deploymentDefinition;
			_mapper = mapper;
			_masterFileName = string.IsNullOrEmpty(masterFileName) ? "apim_deploy.template.json" : masterFileName;
			_fileNamePrefix = string.IsNullOrEmpty(fileNamePrefix) ? string.Empty : fileNamePrefix;
		}

		public async Task CreateAsync()
		{
			var resources = new List<ArmTemplateResource>();

			resources.AddRange(CreateGlobalPolicyResource());

			resources.AddRange(CreateApiVersionSetResource());

			resources.AddRange(CreateCertificateResource());

			resources.AddRange(CreateUserResource());

			resources.AddRange(CreateSubscriptionResource());

			resources.AddRange(CreateProductResource());

			resources.AddRange(CreateTagsResource());

			resources.AddRange(CreateLoggerResource());

			resources.AddRange(CreateBackendResource());

			resources.AddRange(CreateAuthorizationServerResource());

			resources.AddRange(CreateApiSubsequentTemplate());

			OrderResources(resources);

			await SaveMasterTemplate(resources);
		}

		private IEnumerable<ArmTemplateResource> CreateApiSubsequentTemplate()
		{
			Console.WriteLine("Creating api subsequent template");
			Console.WriteLine("------------------------------------------");

			var resources = new List<ArmTemplateResource>();

			resources.AddRange(
				new ArmTemplateResourceCreator<ApiDeploymentDefinition, ApiProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.Apis)
				.WithName(d => d.Name)
				.OfType(ResourceType.Api)
				.CreateResources());

			resources.AddRange(
				new ArmTemplateResourceCreator<ApiDeploymentDefinition, ApiPolicyProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.Apis)
				.WithName(d => $"{d.Name}/policy")
				.OfType(ResourceType.ApiPolicy)
				.WhichDependsOnResourceOfType(ResourceType.Api)
				.WhichDependsOnResourceWithName(d => d.Name)
				.CreateResourcesIf(d => d.HasPolicy()));

			resources.AddRange(
				new ArmTemplateResourceCreator<ApiDeploymentDefinition, ApiOperationPolicyProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.Apis)
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
				.CreateResourcesIf(d => d.Operations != null));

			resources.AddRange(
				new ArmTemplateResourceCreator<ApiDeploymentDefinition, ProductApiProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.Apis)
				.UseResourceCreator(apiDeploymentDefinition =>
				{
					var templateResources = new List<ArmTemplateResource<ProductApiProperties>>();

					foreach (string productDisplayName in apiDeploymentDefinition.ProductList)
					{
						var productName = apiDeploymentDefinition.GetProductName(productDisplayName);

						var templateResource = new ArmTemplateResource<ProductApiProperties>(
							$"{productName}/{apiDeploymentDefinition.Name}",
							$"[concat(parameters('ApimServiceName'), '/{productName}/{apiDeploymentDefinition.Name}')]",
							ResourceType.ProductApi,
							new ProductApiProperties(),
							new[]
							{
								$"[resourceId('{ResourceType.Api}', parameters('ApimServiceName'), '{apiDeploymentDefinition.Name}')]",
								$"[resourceId('{ResourceType.Product}', parameters('ApimServiceName'), '{productName}')]"
							});

						templateResources.Add(templateResource);
					}

					return templateResources;
				})
				.CreateResourcesIf(d => d.IsDependentOnProducts()));


			resources.AddRange(
				new ArmTemplateResourceCreator<ApiDeploymentDefinition, TagApiTemplateProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.Apis)
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
				.CreateResourcesIf(d => d.IsDependentOnTags()));

			return resources;
		}

		private IEnumerable<ArmTemplateResource> CreateAuthorizationServerResource()
		{
			Console.WriteLine("Creating authorization servers template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<AuthorizationServerDeploymentDefinition, AuthorizationServerProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.AuthorizationServers)
				.WithName(d => d.DisplayName)
				.OfType(ResourceType.AuthorizationServer)
				.CreateResources();
		}

		private IEnumerable<ArmTemplateResource> CreateBackendResource()
		{
			Console.WriteLine("Creating backends template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<BackendDeploymentDefinition, BackendProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.Backends)
				.WithName(d => d.Name)
				.OfType(ResourceType.Backend)
				.CreateResources();
		}

		private IEnumerable<ArmTemplateResource> CreateLoggerResource()
		{
			Console.WriteLine("Creating loggers template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<LoggerDeploymentDefinition, LoggerProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.Loggers)
				.WithName(d => d.Name)
				.OfType(ResourceType.Logger)
				.CreateResources();
		}

		private IEnumerable<ArmTemplateResource> CreateTagsResource()
		{
			Console.WriteLine("Creating tags template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<TagDeploymentDefinition, TagProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.Tags)
				.WithName(d => d.Name)
				.OfType(ResourceType.Tag)
				.CreateResources();
		}

		private IEnumerable<ArmTemplateResource> CreateProductResource()
		{
			Console.WriteLine("Creating products template");
			Console.WriteLine("------------------------------------------");

			var resources = new List<ArmTemplateResource>();

			if (_deploymentDefinition.Products.Count() == 0)
			{
				return Array.Empty<ArmTemplateResource>();
			}

			resources.AddRange(new ArmTemplateResourceCreator<ProductDeploymentDefinition, ProductsProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.Products)
				.WithName(d => d.Name)
				.OfType(ResourceType.Product)
				.CreateResources());

			resources.AddRange(
				new ArmTemplateResourceCreator<ProductDeploymentDefinition, ProductPolicyProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.Products)
				.WithName(d => $"{d.Name}/policy")
				.OfType(ResourceType.ProductPolicy)
				.WhichDependsOnResourceOfType(ResourceType.Product)
				.WhichDependsOnResourceWithName(d => d.Name)
				.CreateResourcesIf(d => d.Policy != null));

			resources.AddRange(
				new ArmTemplateResourceCreator<ProductDeploymentDefinition, TagProductProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.Products)
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
				.CreateResourcesIf(d => d.Tags != null));

			return resources;
		}

		private IEnumerable<ArmTemplateResource> CreateSubscriptionResource()
		{
			Console.WriteLine("Creating subscriptions template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<SubscriptionDeploymentDefinition, SubscriptionProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.Subscriptions)
				.WithName(d => d.Name)
				.OfType(ResourceType.Subscription)
				.CreateResources();
		}

		private IEnumerable<ArmTemplateResource> CreateUserResource()
		{
			Console.WriteLine("Creating users template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<UserDeploymentDefinition, UserProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.Users)
				.WithName(d => d.Name)
				.OfType(ResourceType.User)
				.CreateResources();
		}

		private IEnumerable<ArmTemplateResource> CreateCertificateResource()
		{
			Console.WriteLine("Creating certificates template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<CertificateDeploymentDefinition, CertificateProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.Certificates)
				.WithName(d => d.Name)
				.OfType(ResourceType.Certificate)
				.CreateResources();
		}

		private IEnumerable<ArmTemplateResource> CreateApiVersionSetResource()
		{
			Console.WriteLine("Creating api version sets template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<ApiVersionSetDeploymentDefinition, ApiVersionSetProperties>(_mapper)
				.ForDeploymentDefinitions(_deploymentDefinition.ApiVersionSets)
				.WithName(d => d.Name)
				.OfType(ResourceType.ApiVersionSet)
				.CreateResources();
		}

		private IEnumerable<ArmTemplateResource> CreateGlobalPolicyResource()
		{
			Console.WriteLine("Creating global service policy template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<DeploymentDefinition, PolicyProperties>(_mapper)
				.ForDeploymentDefinition(_deploymentDefinition)
				.WithName((_) => "/policy")
				.OfType(ResourceType.GlobalServicePolicy)
				.CreateResourcesIf(d => d.Policy != null);
		}

		private async Task SaveMasterTemplate(List<ArmTemplateResource> resources)
		{
			Console.WriteLine("Creating deploy template");
			Console.WriteLine("------------------------------------------");

			var masterTemplateCreator = new DeployArmTemplateCreator();

			var masterTemplate = await masterTemplateCreator.Create(resources);
			SaveTemplate($"{_fileNamePrefix}{_masterFileName}", masterTemplate);

			var templateParameters = masterTemplateCreator.CreateMasterTemplateParameterValues(_deploymentDefinition);
			SaveTemplate($"{_fileNamePrefix}parameters.json", templateParameters);

			Console.WriteLine("Templates written to output location");
		}

		private void OrderResources(List<ArmTemplateResource> resources)
		{
			AddDependency<BackendProperties, CertificateProperties>(resources);
			AddDependency<PolicyProperties, CertificateProperties>(resources);

			AddDependency<ApiProperties, ApiVersionSetProperties>(resources);
			AddDependency<ApiProperties, TagProperties>(resources);
			AddDependency<ApiProperties, LoggerProperties>(resources);
			AddDependency<ApiProperties, BackendProperties>(resources);
			AddDependency<ApiProperties, AuthorizationServerProperties>(resources);
			AddDependency<ApiProperties, CertificateProperties>(resources);

			AddDependency<ProductsProperties, CertificateProperties>(resources);
			AddDependency<ProductsProperties, ApiProperties>(resources);
			AddDependency<ApiProperties, TagProductProperties>(resources);

			AddDependency<SubscriptionProperties, ApiProperties>(resources);
			AddDependency<SubscriptionProperties, ProductsProperties>(resources);
			AddDependency<SubscriptionProperties, UserProperties>(resources);
		}

		private void AddDependency<TDependentResource, TDependencyResource>(List<ArmTemplateResource> resources)
		{
			var dependentResources = GetResources<TDependentResource>(resources);
			var dependencyResources = GetResources<TDependencyResource>(resources);

			AddDependencyTo(dependentResources, dependencyResources);
		}

		private void AddDependencyTo(IEnumerable<ArmTemplateResource> resources, IEnumerable<ArmTemplateResource> dependencies)
		{
			foreach (var resource in resources)
			{
				if (dependencies.Contains(resource))
				{
					continue;
				}

				resource.AddDependencies(dependencies);
			}
		}

		private IEnumerable<ArmTemplateResource> GetResources<TResourceProperties>(List<ArmTemplateResource> resources)
		{
			return resources.Where(resources => resources is ArmTemplateResource<TResourceProperties>);
		}

		private void SaveTemplate(string fileName, ArmTemplate template)
		{
			var path = Path.Combine(_deploymentDefinition.OutputLocation, fileName);
			_fileWriter.WriteJson(template, path);
		}
	}
}
