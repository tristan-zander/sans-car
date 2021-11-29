// Licensed under the Mozilla Public License 2.0.
// Usage of these files must be in agreement with the license.
//
// You may find a copy of the license at https://www.mozilla.org/en-US/MPL/2.0/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Net;

namespace Data
{
	public class User
	{
		/// <summary>
		/// Analogous to the Discord user's ID snowflake.
		/// </summary>
		[Key] public ulong Id { get; init; }

		public User(DiscordUser user)
		{
			Id = user.Id;
		}

		public User(ulong id)
		{
			Id = id;
		}
	}
}
