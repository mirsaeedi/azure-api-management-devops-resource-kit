using Apim.DevOps.Toolkit.ApimEntities.User;
using System;
using System.Collections.Generic;

namespace Apim.DevOps.Toolkit.Core.DeploymentDefinitions.ApimEntities
{
	public class UserDeploymentDefinition : EntityDeploymentDefinition
	{
		/// <summary>
		/// The Id of the User
		/// </summary>
		public string Name { get; set; }

		public UserState? State { get; set; }

		public string Note { get; set; }

		public UserIdentityContract[] Identities { get; set; }

		public string Email { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Password { get; set; }

		public UserConfirmation? Confirmation { get; set; }

		public override IEnumerable<string> Dependencies() => Array.Empty<string>();
	}
}