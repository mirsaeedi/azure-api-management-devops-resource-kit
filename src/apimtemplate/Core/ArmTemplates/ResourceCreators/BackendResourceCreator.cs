using Apim.DevOps.Toolkit.ApimEntities.Backend;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates.ResourceCreators
{
	public class BackendResourceCreator : IResourceCreator
	{
		private IMapper _mapper;

		public BackendResourceCreator(IMapper mapper)
		{
			_mapper = mapper;
		}
		public IEnumerable<ArmTemplateResource> Create(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.Backends.Count() == 0)
			{
				return Array.Empty<ArmTemplateResource>();
			}

			Console.WriteLine("Creating backends template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<BackendDeploymentDefinition, BackendProperties>(_mapper)
				.ForDeploymentDefinitions(deploymentDefinition.Backends)
				.WithName(d => d.Name)
				.OfType(ResourceType.Backend)
				.CreateResources();
		}
	}
}
