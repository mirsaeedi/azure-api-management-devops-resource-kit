
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class CertificateTemplateResource : TemplateResource<CertificateProperties>
    {
        public override string Type => ResourceType.Certificate;
    }
}
