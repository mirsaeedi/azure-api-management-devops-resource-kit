using System;
using System.Collections.Generic;
using System.Text;


namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class ApiVersionSetProperties
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string VersionQueryName { get; set; }
        public string VersionHeaderName { get; set; }
        public string VersioningScheme { get; set; }
    }
}
