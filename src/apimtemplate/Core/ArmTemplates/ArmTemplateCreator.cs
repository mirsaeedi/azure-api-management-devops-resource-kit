using Apim.DevOps.Toolkit.ApimEntities.Api;
using Apim.DevOps.Toolkit.ApimEntities.AuthotizationServer;
using Apim.DevOps.Toolkit.ApimEntities.Backend;
using Apim.DevOps.Toolkit.ApimEntities.Logger;
using Apim.DevOps.Toolkit.ApimEntities.NamedValues;
using Apim.DevOps.Toolkit.ApimEntities.Policy;
using Apim.DevOps.Toolkit.ApimEntities.Product;
using Apim.DevOps.Toolkit.ArmTemplates;
using Apim.DevOps.Toolkit.Core.ArmTemplates.ResourceCreators;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates
{
	public class ArmTemplateCreator
	{
		private readonly DeploymentDefinition _deploymentDefinition;
		private readonly IMapper _mapper;

		public ArmTemplateCreator(DeploymentDefinition deploymentDefinition, IMapper mapper)
		{
			_deploymentDefinition = deploymentDefinition;
			_mapper = mapper;
		}

		public List<ArmTemplateResource> Create()
		{
			var resources = new List<ArmTemplateResource>();

			foreach (var resourceCreator in DiscoverResourceCreators(_mapper))
			{
				resources.AddRange(resourceCreator.Create(_deploymentDefinition));
			}

			OrderResources(resources);

			return resources;
		}

		private void OrderResources(List<ArmTemplateResource> resources)
		{
			AddDependency<BackendProperties, CertificateProperties>(resources);
			AddDependency<BackendProperties, NamedValueProperties>(resources);

			AddDependency<PolicyProperties, CertificateProperties>(resources);
			AddDependency<PolicyProperties, NamedValueProperties>(resources);

			AddDependency<ApiProperties, LoggerProperties>(resources);
			AddDependency<ApiProperties, BackendProperties>(resources);
			AddDependency<ApiProperties, AuthorizationServerProperties>(resources);
			AddDependency<ApiProperties, CertificateProperties>(resources);
			AddDependency<ApiProperties, NamedValueProperties>(resources);

			AddDependency<ProductsProperties, CertificateProperties>(resources);
			AddDependency<ProductsProperties, NamedValueProperties>(resources);
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

		private IEnumerable<IResourceCreator> DiscoverResourceCreators(IMapper mapper)
		{
			return Assembly
				.GetExecutingAssembly()
				.GetTypes()
				.Where(x => typeof(IResourceCreator).IsAssignableFrom(x) && !x.IsInterface)
				.Select(x => (IResourceCreator)Activator.CreateInstance(x, new[] { mapper }));

		}
	}
}
