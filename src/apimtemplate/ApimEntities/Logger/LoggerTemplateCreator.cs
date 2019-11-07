using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Apim.DevOps.Toolkit.ArmTemplates;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class LoggerTemplateCreator : TemplateCreator, ITemplateCreator
    {
        public async Task<Template> Create(DeploymentDefinition creatorConfig)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();
            foreach (LoggerDeploymentDefinition logger in creatorConfig.Loggers)
            {
                var loggerTemplateResource = new LoggerTemplateResource()
                {
                    Name = $"[concat(parameters('ApimServiceName'), '/{logger.Name}')]",
                    Properties = new LoggerProperties()
                    {
                        LoggerType = logger.LoggerType,
                        Description = logger.Description,
                        Credentials = logger.Credentials,
                        IsBuffered = logger.IsBuffered,
                        ResourceId = logger.ResourceId
                    },
                    DependsOn = new string[] { }
                };
                resources.Add(loggerTemplateResource);
            }

            template.Resources = resources.ToArray();
            return await Task.FromResult(template);
        }
    }
}
