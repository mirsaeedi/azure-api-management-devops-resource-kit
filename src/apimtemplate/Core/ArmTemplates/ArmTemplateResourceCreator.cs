using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates
{
	public class ArmTemplateResourceCreator<TResourceDeploymentDefinition, TResourceProperties> where TResourceDeploymentDefinition: EntityDeploymentDefinition
	{
		private readonly IMapper _mapper;
		private readonly List<TResourceDeploymentDefinition> _resourceDeploymentDefinitions = new List<TResourceDeploymentDefinition>();

		private string _resourceType;
		private Func<TResourceDeploymentDefinition, string> _getResourceName;
		private string _parentResourceType;
		private Func<TResourceDeploymentDefinition, string> _getParentResourceName;
		private Func<TResourceDeploymentDefinition, IEnumerable<ArmTemplateResource<TResourceProperties>>> _resourceCreator;
		private bool _checkDependencies;

		public ArmTemplateResourceCreator(IMapper mapper)
		{
			_mapper = mapper;
		}

		public ArmTemplateResourceCreator<TResourceDeploymentDefinition, TResourceProperties> ForDeploymentDefinitions(IEnumerable<TResourceDeploymentDefinition> resourceDeploymentDefinitions)
		{
			_resourceDeploymentDefinitions.AddRange(resourceDeploymentDefinitions);
			return this;
		}

		public ArmTemplateResourceCreator<TResourceDeploymentDefinition, TResourceProperties> ForDeploymentDefinition(TResourceDeploymentDefinition resourceDeploymentDefinition)
		{
			_resourceDeploymentDefinitions.Add(resourceDeploymentDefinition);
			return this;
		}

		public ArmTemplateResourceCreator<TResourceDeploymentDefinition, TResourceProperties> OfType(string resourceType)
		{
			_resourceType = resourceType;
			return this;
		}

		public ArmTemplateResourceCreator<TResourceDeploymentDefinition, TResourceProperties> WithName(Func<TResourceDeploymentDefinition, string> getResourceName)
		{
			_getResourceName = getResourceName;
			return this;
		}

		public ArmTemplateResourceCreator<TResourceDeploymentDefinition, TResourceProperties> CheckDependencies()
		{
			_checkDependencies = true;
			return this;
		}

		public ArmTemplateResourceCreator<TResourceDeploymentDefinition, TResourceProperties> WhichDependsOnResourceOfType(string parentResourceType)
		{
			_parentResourceType = parentResourceType;
			return this;
		}

		public ArmTemplateResourceCreator<TResourceDeploymentDefinition, TResourceProperties> WhichDependsOnResourceWithName(Func<TResourceDeploymentDefinition, string> getParentResourceName)
		{
			_getParentResourceName = getParentResourceName;
			return this;
		}

		public ArmTemplateResourceCreator<TResourceDeploymentDefinition, TResourceProperties> UseResourceCreator(Func<TResourceDeploymentDefinition, IEnumerable<ArmTemplateResource<TResourceProperties>>> resourceCreator)
		{
			_resourceCreator = resourceCreator;
			return this;
		}

		public IEnumerable<ArmTemplateResource<TResourceProperties>> CreateResourcesIf(Predicate<TResourceDeploymentDefinition> shouldConsider, bool sequential = false)
		{
			if (_resourceDeploymentDefinitions.Count() == 0)
			{
				return null;
			}

			var resources = new List<ArmTemplateResource<TResourceProperties>>();
			var previousResource = default(ArmTemplateResource<TResourceProperties>);

			foreach (var deploymentDefinition in _resourceDeploymentDefinitions)
			{
				if (!shouldConsider(deploymentDefinition))
				{
					continue;
				}

				var newResources = _resourceCreator != null ? _resourceCreator(deploymentDefinition) : new[] { GetResource(_getResourceName, _resourceType, deploymentDefinition) };

				foreach (var newResource in newResources)
				{
					if (previousResource != null)
					{
						newResource.AddDependencies(new[] { previousResource });
					}

					if (sequential)
					{
						previousResource = newResource;
					}
				}

				resources.AddRange(newResources);
			}

			return resources;
		}

		public IEnumerable<ArmTemplateResource<TResourceProperties>> CreateResources(bool sequential = false)
		{
			return CreateResourcesIf((_) => true, sequential);
		}

		private ArmTemplateResource<TResourceProperties> GetResource(
			Func<TResourceDeploymentDefinition, string> getResourceName,
			string resourceType,
			TResourceDeploymentDefinition deploymentDefinition)
		{
			var name = getResourceName(deploymentDefinition).Trim(new[] { '/' });

			return new ArmTemplateResource<TResourceProperties>(
					name,
					$"[concat(parameters('ApimServiceName'), '/{name}')]",
					resourceType,
					_mapper.Map<TResourceProperties>(deploymentDefinition),
					GetDependsOn(deploymentDefinition));
		}

		private IEnumerable<string> GetDependsOn(TResourceDeploymentDefinition deploymentDefinition)
		{
			var dependencies = new List<string>();

			if (_parentResourceType != null && _getParentResourceName != null)
			{
				dependencies.Add($"[resourceId('{_parentResourceType}', parameters('ApimServiceName'), '{_getParentResourceName(deploymentDefinition)}')]");
			}

			if (_checkDependencies)
			{
				dependencies.AddRange(deploymentDefinition.Dependencies());
			}

			return dependencies;
		}
	}
}
