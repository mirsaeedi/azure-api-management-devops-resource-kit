using Apim.DevOps.Toolkit.Core.ArmTemplates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apim.DevOps.Toolkit.ApimEntities.Api
{
    public class ApiUpdateDisplayNameProperties
    {
        public string Mode { get; set; } = "Incremental";
        public ArmTemplate Template { get; set; }
    }
}
