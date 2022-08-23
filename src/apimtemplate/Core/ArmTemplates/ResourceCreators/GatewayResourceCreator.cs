using Apim.DevOps.Toolkit.ApimEntities.Gateway;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions;
using Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities;
using Apim.DevOps.Toolkit.Core.Infrastructure.Constants;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apim.DevOps.Toolkit.Core.ArmTemplates.ResourceCreators
{
    public class GatewayResourceCreator : IResourceCreator
    {
        private IMapper _mapper;

        public GatewayResourceCreator(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IEnumerable<ArmTemplateResource> Create(DeploymentDefinition deploymentDefinition)
        {
            if (deploymentDefinition.Gateways.Count() == 0)
            {
                return Array.Empty<ArmTemplateResource>();
            }

            Console.WriteLine("Creating gateways template");
            Console.WriteLine("------------------------------------------");

            return new ArmTemplateResourceCreator<GatewayDeploymentDefinition, GatewayProperties>(_mapper)
                .ForDeploymentDefinitions(deploymentDefinition.Gateways)
                .WithName(d => d.Name)
                .OfType(ResourceType.Gateway)
                .CreateResources();
        }
    }
}
