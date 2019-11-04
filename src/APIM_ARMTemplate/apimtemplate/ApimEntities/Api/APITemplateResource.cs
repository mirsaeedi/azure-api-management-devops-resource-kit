
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System;

namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class ApiTemplateResource: TemplateResource<ApiProperties>
    {
        public TemplateResource[] Resources { get; set; }
        public override string Type => ResourceType.Api;
    }
}