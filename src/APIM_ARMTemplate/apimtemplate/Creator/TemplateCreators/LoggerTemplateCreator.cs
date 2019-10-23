using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class LoggerTemplateCreator : TemplateCreator, ITemplateCreator
    {
        public async Task<Template> Create(CreatorConfig creatorConfig)
        {
            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();
            foreach (LoggerConfig logger in creatorConfig.Loggers)
            {
                var loggerTemplateResource = new LoggerTemplateResource()
                {
                    Name = $"[concat(parameters('ApimServiceName'), '/{logger.Name}')]",
                    Properties = new LoggerTemplateProperties()
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

            template.resources = resources.ToArray();
            return await Task.FromResult(template);
        }
    }
}
