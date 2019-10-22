using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apim.Arm.Creator.Creator.TemplateCreators
{
    interface ITemplateCreator
    {
        Template Create(CreatorConfig creatorConfig);
    }
}
