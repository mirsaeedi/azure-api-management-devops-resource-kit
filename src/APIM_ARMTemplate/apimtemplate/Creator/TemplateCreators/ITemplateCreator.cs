using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Apim.Arm.Creator.Creator.TemplateCreators
{
    interface ITemplateCreator
    {
        Task<Template> Create(CreatorConfig creatorConfig);
    }
}
