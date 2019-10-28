
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class LoggerTemplateResource : TemplateResource<LoggerProperties>
    {
        public override string Type => ResourceType.Logger;
    }

   
}
