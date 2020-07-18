using Newtonsoft.Json;

namespace Apim.DevOps.Toolkit.ApimEntities.User
{
	public class UserProperties
	{
		public UserState? State { get; set; }

		public string Note { get; set; }

		public UserIdentityContract[] Identities { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Email { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string FirstName { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string LastName { get; set; }

		public string Password { get; set; }

		public UserConfirmation? Confirmation { get; set; }
	}
}
