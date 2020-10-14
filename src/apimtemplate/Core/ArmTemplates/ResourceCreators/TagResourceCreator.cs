using Apim.DevOps.Toolkit.ApimEntities.Tag;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates.ResourceCreators
{
	public class TagResourceCreator : IResourceCreator
	{
		private IMapper _mapper;

		public TagResourceCreator(IMapper mapper)
		{
			_mapper = mapper;
		}
		public IEnumerable<ArmTemplateResource> Create(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.Tags.Count() == 0)
			{
				return Array.Empty<ArmTemplateResource>();
			}

			Console.WriteLine("Creating tags template");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<TagDeploymentDefinition, TagProperties>(_mapper)
				.ForDeploymentDefinitions(deploymentDefinition.Tags)
				.WithName(d => d.Name)
				.OfType(ResourceType.Tag)
				.CreateResources();
		}
	}
}
