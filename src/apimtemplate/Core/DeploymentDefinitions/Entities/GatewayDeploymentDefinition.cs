using Apim.DevOps.Toolkit.ApimEntities.Gateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
