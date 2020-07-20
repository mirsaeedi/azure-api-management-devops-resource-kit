using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.Templates
{
	public class ArmTemplateResourceCreator<TResourceDeploymentDefinition, TResourceProperties>
	{
		private readonly IMapper _mapper;
		private readonly List<TResourceDeploymentDefinition> _resourceDeploymentDefinitions = new List<TResourceDeploymentDefinition>();

		private string _resourceType;
		private Func<TResourceDeploymentDefinition, string> _getResourceName;
		private string _parentResourceType;
		private Func<TResourceDeploymentDefinition, string> _getParentResourceName;
		private Func<TResourceDeploymentDefinition, IEnumerable<ArmTemplateResource<TResourceProperties>>> _resourceCreator;

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

		public IEnumerable<ArmTemplateResource> CreateResourcesIf(Predicate<TResourceDeploymentDefinition> shouldConsider)
		{
			var resources = new List<ArmTemplateResource>();

			if (_resourceDeploymentDefinitions.Count() == 0)
			{
				return null;
			}

			foreach (var deploymentDefinition in _resourceDeploymentDefinitions)
			{
				if (!shouldConsider(deploymentDefinition))
				{
					continue;
				}

				resources.AddRange(_resourceCreator != null ? _resourceCreator(deploymentDefinition) : GetResource(_getResourceName, _resourceType, deploymentDefinition));
			}

			return resources;
		}

		public IEnumerable<ArmTemplateResource<TResourceProperties>> CreateResources()
		{
			var resources = new List<ArmTemplateResource<TResourceProperties>>();
			foreach (var deploymentDefinition in _resourceDeploymentDefinitions)
			{
				resources.AddRange(GetResource(_getResourceName, _resourceType, deploymentDefinition));
			}

			return resources;
		}

		private IEnumerable<ArmTemplateResource<TResourceProperties>> GetResource(
			Func<TResourceDeploymentDefinition, string> getResourceName,
			string resourceType,
			TResourceDeploymentDefinition deploymentDefinition)
		{
			var name = getResourceName(deploymentDefinition).Trim(new [] { '/' });

			return new[]
			{
				new ArmTemplateResource<TResourceProperties>(
					name,
					$"[concat(parameters('ApimServiceName'), '/{name}')]",
					resourceType,
					_mapper.Map<TResourceProperties>(deploymentDefinition),
					GetDependsOn(deploymentDefinition))
			};
		}

		private IEnumerable<string> GetDependsOn(TResourceDeploymentDefinition deploymentDefinition)
		{
			if (_parentResourceType == null || _getParentResourceName == null)
			{
				return Array.Empty<string>();
			}

			return new[] { $"[resourceId('{_parentResourceType}', parameters('ApimServiceName'), '{_getParentResourceName(deploymentDefinition)}')]" };
		}
	}
}
