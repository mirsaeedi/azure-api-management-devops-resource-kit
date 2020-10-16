using Apim.DevOps.Toolkit.ApimEntities.NamedValues;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates.ResourceCreators
{
	public class NamedValueResourceCreator : IResourceCreator
	{
		private IMapper _mapper;

		public NamedValueResourceCreator(IMapper mapper)
		{
			_mapper = mapper;
		}
		public IEnumerable<ArmTemplateResource> Create(DeploymentDefinition deploymentDefinition)
		{
			if (deploymentDefinition.NamedValues.Count() == 0)
			{
				return Array.Empty<ArmTemplateResource>();
			}

			Console.WriteLine("Creating named values resources");
			Console.WriteLine("------------------------------------------");

			return new ArmTemplateResourceCreator<NamedValueDeploymentDefinition, NamedValueProperties>(_mapper)
				.ForDeploymentDefinitions(deploymentDefinition.NamedValues)
				.WithName(d => d.Name)
				.OfType(ResourceType.NameValue)
				.CreateResources();
		}
	}
}
