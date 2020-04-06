using System.Collections.Generic;
using System.Threading.Tasks;
using Apim.Arm.Creator.Creator.TemplateCreators;
using Apim.DevOps.Toolkit.ArmTemplates;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Create
{
    public class CertificateTemplateCreator : TemplateCreator, ITemplateCreator
    {
        public async Task<Template> Create(DeploymentDefinition creatorConfig)
        {
			var fileReader = new FileReader();

            var template = EmptyTemplate;
            template.Parameters.Add(ApiServiceNameParameter.Key, ApiServiceNameParameter.Value);

            var resources = new List<TemplateResource>();

            foreach (var certificateTemplatePropeties in creatorConfig.Certificates)
            {
				certificateTemplatePropeties.Data = await fileReader.RetrieveFileContentsAsync(certificateTemplatePropeties.FilePath, convertToBase64:true);

                var certificateTemplateResource = new CertificateTemplateResource()
                {
                    Name = $"[concat(parameters('ApimServiceName'), '/{certificateTemplatePropeties.Name}')]",
                    Properties = new CertificateProperties()
					{
						Data = certificateTemplatePropeties.Data,
						Password = certificateTemplatePropeties.Password
					},
                    DependsOn = new string[] { }
                };
                resources.Add(certificateTemplateResource);
            }

            template.Resources = resources.ToArray();
            return await Task.FromResult(template);
        }
	}
}
