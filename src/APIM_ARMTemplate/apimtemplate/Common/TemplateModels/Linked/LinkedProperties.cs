﻿using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class LinkedProperties
    {
        public string Mode { get; set; }
        public LinkedTemplateLink TemplateLink { get; set; }
        public Dictionary<string, TemplateParameterProperties> Parameters { get; set; }
    }

    public class LinkedTemplateLink
    {
        public string Uri { get; set; }
        public string ContentVersion { get; set; }
    }
}
