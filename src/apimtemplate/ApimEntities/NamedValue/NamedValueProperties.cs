using System.Collections.Generic;
using Apim.DevOps.Toolkit.Extensions;

namespace Apim.DevOps.Toolkit.ApimEntities
{
    public class NamedValueProperties
    {
        public bool Secret { get; set; }

        public string DisplayName { get; set; }

        public string Value { get; set; }

        public string Name { get; set; }
    }
}
