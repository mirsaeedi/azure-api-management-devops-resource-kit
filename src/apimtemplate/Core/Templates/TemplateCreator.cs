using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.Templates
{
	public class TemplateCreator<TResourceDeploymentDefinition, TResourceProperties>
	{
		private readonly IMapper _mapper;
		private readonly List<TResourceDeploymentDefinition> _resourceDeploymentDefinitions = new List<TResourceDeploymentDefinition>();

		private string _resourceType;
		private Func<TResourceDeploymentDefinition, string> _getResourceName;
		private string _parentResourceType;
		private Func<TResourceDeploymentDefinition, string> _getParentResourceName;
		private Func<TResourceDeploymentDefinition, IEnumerable<TemplateResource<TResourceProperties>>> _resourceCreator;

		public TemplateCreator(IMapper mapper)
		{
			_mapper = mapper;
		}

		public TemplateCreator<TResourceDeploymentDefinition, TResourceProperties> ForDeploymentDefinitions(IEnumerable<TResourceDeploymentDefinition> resourceDeploymentDefinitions)
		{
			_resourceDeploymentDefinitions.AddRange(resourceDeploymentDefinitions);
			return this;
		}

		public TemplateCreator<TResourceDeploymentDefinition, TResourceProperties> ForDeploymentDefinition(TResourceDeploymentDefinition resourceDeploymentDefinition)
		{
			_resourceDeploymentDefinitions.Add(resourceDeploymentDefinition);
			return this;
		}

		public TemplateCreator<TResourceDeploymentDefinition, TResourceProperties> OfType(string resourceType)
		{
			_resourceType = resourceType;
			return this;
		}

		public TemplateCreator<TResourceDeploymentDefinition, TResourceProperties> WithName(Func<TResourceDeploymentDefinition, string> getResourceName)
		{
			_getResourceName = getResourceName;
			return this;
		}

		public TemplateCreator<TResourceDeploymentDefinition, TResourceProperties> WhichDependsOnResourceOfType(string parentResourceType)
		{
			_parentResourceType = parentResourceType;
			return this;
		}

		public TemplateCreator<TResourceDeploymentDefinition, TResourceProperties> WhichDependsOnResourceWithName(Func<TResourceDeploymentDefinition, string> getParentResourceName)
		{
			_getParentResourceName = getParentResourceName;
			return this;
		}

		public TemplateCreator<TResourceDeploymentDefinition, TResourceProperties> UseResourceCreator(Func<TResourceDeploymentDefinition, IEnumerable<TemplateResource<TResourceProperties>>> resourceCreator)
		{
			_resourceCreator = resourceCreator;
			return this;
		}

		public IEnumerable<TemplateResource> CreateResourcesIf(Predicate<TResourceDeploymentDefinition> shouldConsider)
		{
			var resources = new List<TemplateResource>();

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

		public IEnumerable<TemplateResource<TResourceProperties>> CreateResources()
		{
			var resources = new List<TemplateResource<TResourceProperties>>();
			foreach (var deploymentDefinition in _resourceDeploymentDefinitions)
			{
				resources.AddRange(GetResource(_getResourceName, _resourceType, deploymentDefinition));
			}

			return resources;
		}

		private IEnumerable<TemplateResource<TResourceProperties>> GetResource(
			Func<TResourceDeploymentDefinition, string> getResourceName,
			string resourceType,
			TResourceDeploymentDefinition deploymentDefinition)
		{
			var name = getResourceName(deploymentDefinition).Trim(new [] { '/' });

			return new[]
			{
				new TemplateResource<TResourceProperties>(
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
