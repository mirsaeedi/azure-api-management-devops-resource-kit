using Apim.DevOps.Toolkit.ApimEntities.Policy;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using AutoMapper;
using System;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates.ResourceCreators
{
	public class GlobalPolicyResourceCreator : IResourceCreator
	{
		private IMapper _mapper;

		public GlobalPolicyResourceCreator(IMapper mapper)
		{
			_mapper = mapper;
		}
		public IEnumerable<ArmTemplateResource> Create(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.Policy != null)
			{
				return Array.Empty<ArmTemplateResource>();
			}

			Console.WriteLine("Creating global service policy template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<DeploymentDefinition, PolicyProperties>(_mapper)
				.ForDeploymentDefinition(deploymentDefinition)
				.WithName((_) => "/policy")
				.OfType(ResourceType.GlobalServicePolicy)
				.CreateResourcesIf(d => d.Policy != null);
		}
	}
}
