using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apim.DevOps.Toolkit.ArmTemplates
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

	public class UserIdentityContract
	{
		public string Provider { get; set; }

		public string Id { get; set; }
	}

	public enum UserConfirmation
	{
		Signup,
		Invite
	}

	public enum UserState
	{
		Active,
		Blocked,
		Pending,
		Deleted
	}
}
