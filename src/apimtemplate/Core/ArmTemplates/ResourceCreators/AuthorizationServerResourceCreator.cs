using Apim.DevOps.Toolkit.ApimEntities.AuthotizationServer;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates.ResourceCreators
{
	public class AuthorizationServerResourceCreator : IResourceCreator
	{
		private IMapper _mapper;

		public AuthorizationServerResourceCreator(IMapper mapper)
		{
			_mapper = mapper;
		}
		public IEnumerable<ArmTemplateResource> Create(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.AuthorizationServers.Count() == 0)
			{
				return Array.Empty<ArmTemplateResource>();
			}

			Console.WriteLine("Creating authorization servers template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<AuthorizationServerDeploymentDefinition, AuthorizationServerProperties>(_mapper)
				.ForDeploymentDefinitions(deploymentDefinition.AuthorizationServers)
				.WithName(d => d.DisplayName)
				.OfType(ResourceType.AuthorizationServer)
				.CreateResources();
		}
	}
}
