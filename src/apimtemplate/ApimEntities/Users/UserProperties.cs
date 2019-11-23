using System;
using System.Collections.Generic;
using System.Text;

namespace Apim.DevOps.Toolkit.ArmTemplates
{
    public class UserProperties
    {
        public string State { get; set; }

        public string Note { get; set; }

		public UserIdentityContract[] Identities { get; set; }

		public string Email { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Password { get; set; }

		public UserConfirmation Confirmation { get; set; }
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
}
