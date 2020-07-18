namespace Apim.DevOps.Toolkit.ApimEntities.Api
{

	public class ApiTemplateAuthenticationSettings
	{
		public ApiTemplateOAuth2 OAuth2 { get; set; }
		public ApiTemplateOpenID OpenId { get; set; }
		public bool SubscriptionKeyRequired { get; set; }
	}
}
