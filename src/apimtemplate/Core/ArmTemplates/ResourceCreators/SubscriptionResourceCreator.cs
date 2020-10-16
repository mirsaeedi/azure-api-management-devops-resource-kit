using Apim.DevOps.Toolkit.ApimEntities.Subscription;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates.ResourceCreators
{
	public class SubscriptionResourceCreator : IResourceCreator
	{
		private IMapper _mapper;

		public SubscriptionResourceCreator(IMapper mapper)
		{
			_mapper = mapper;
		}
		public IEnumerable<ArmTemplateResource> Create(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.Subscriptions.Count() == 0)
			{
				return Array.Empty<ArmTemplateResource>();
			}

			Console.WriteLine("Creating subscriptions template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<SubscriptionDeploymentDefinition, SubscriptionProperties>(_mapper)
				.ForDeploymentDefinitions(deploymentDefinition.Subscriptions)
				.WithName(d => d.Name)
				.OfType(ResourceType.Subscription)
				.CheckDependencies()
				.CreateResources(true);
		}
	}
}
