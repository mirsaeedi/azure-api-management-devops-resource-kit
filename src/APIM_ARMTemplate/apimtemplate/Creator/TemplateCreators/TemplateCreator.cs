using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;
using System.Collections.Generic;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class TemplateCreator
    {
        public static Template EmptyTemplate => new Template()
        {
            schema = "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
                contentVersion = "1.0.0.0",
                Parameters = new Dictionary<string, TemplateParameterProperties>(),
                variables = { },
                resources = new TemplateResource[] { },
                outputs = { }
        };


        public static KeyValuePair<string, TemplateParameterProperties> ApiServiceNameParameter => new KeyValuePair<string, TemplateParameterProperties>
            ("ApimServiceName", new TemplateParameterProperties() { type = "string" });
    }
}
