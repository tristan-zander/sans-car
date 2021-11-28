// Licensed under the Mozilla Public License 2.0.
// Usage of these files must be in agreement with the license.
//
// You may find a copy of the license at https://www.mozilla.org/en-US/MPL/2.0/

using System;
using System.ComponentModel.DataAnnotations;
using DSharpPlus;
using DSharpPlus.Entities;

namespace Data
{
	public class Channel
	{
		[Key] public ulong Id { get; set; }
	}
}
