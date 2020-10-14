using Apim.DevOps.Toolkit.ApimEntities.User;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates.ResourceCreators
{
	public class UserResourceCreator : IResourceCreator
	{
		private IMapper _mapper;

		public UserResourceCreator(IMapper mapper)
		{
			_mapper = mapper;
		}
		public IEnumerable<ArmTemplateResource> Create(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.Users.Count() == 0)
			{
				return Array.Empty<ArmTemplateResource>();
			}

			Console.WriteLine("Creating users template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<UserDeploymentDefinition, UserProperties>(_mapper)
				.ForDeploymentDefinitions(deploymentDefinition.Users)
				.WithName(d => d.Name)
				.OfType(ResourceType.User)
				.CreateResources();
		}
	}
}
