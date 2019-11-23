
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common;

namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class SubscriptionTemplateResource : TemplateResource<SubscriptionProperties>
    {
        public override string Type => ResourceType.Subscription;
    }
}
