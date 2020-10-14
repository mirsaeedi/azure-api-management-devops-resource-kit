using Apim.DevOps.Toolkit.ApimEntities.ApiVersionSet;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates.ResourceCreators
{
	public class ApiVersionSetResourceCreator : IResourceCreator
	{
		private IMapper _mapper;

		public ApiVersionSetResourceCreator(IMapper mapper)
		{
			_mapper = mapper;
		}
		public IEnumerable<ArmTemplateResource> Create(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.ApiVersionSets.Count() == 0)
			{
				return Array.Empty<ArmTemplateResource>();
			}

			Console.WriteLine("Creating api version sets template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<ApiVersionSetDeploymentDefinition, ApiVersionSetProperties>(_mapper)
				.ForDeploymentDefinitions(deploymentDefinition.ApiVersionSets)
				.WithName(d => d.Name)
				.OfType(ResourceType.ApiVersionSet)
				.CreateResources();
		}
	}
}
