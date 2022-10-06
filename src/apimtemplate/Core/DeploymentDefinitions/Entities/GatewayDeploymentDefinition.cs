using Apim.DevOps.Toolkit.ApimEntities.Gateway;
using System;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.Entities
{
   public class GatewayDeploymentDefinition : EntityDeploymentDefinition
   {
      public string Name { get; set; }

      public string Description { get; set; }

      public GatewayLocationData LocationData { get; set; }

      public override IEnumerable<string> Dependencies() => Array.Empty<string>();
   }
}
