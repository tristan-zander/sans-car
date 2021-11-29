// Licensed under the Mozilla Public License 2.0.
// Usage of these files must be in agreement with the license.
//
// You may find a copy of the license at https://www.mozilla.org/en-US/MPL/2.0/

using System;
using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using NpgsqlTypes;

namespace Data
{
	public class Quote
	{
		[Key] public Guid QuoteId { get; set; } = Guid.NewGuid();

		[Required]
		public Guild Guild { get; set; }
		[Required, MaxLength(500)]
		public string Message { get; set; }

		[Required]
		public DateTimeOffset TimeAdded { get; set; }

		/// <summary>
		/// The user that submitted the quote.
		/// </summary>
		[Required]
		public User BlamedUser { get; set; }

		public Quote()
		{
		}

		public Quote(Quote other, User user = null, Guild guild = null)
		{
			QuoteId = other.QuoteId;
			Guild = guild ?? other.Guild;
			Message = other.Message;
			TimeAdded = other.TimeAdded;
			BlamedUser = user ?? other.BlamedUser;
		}

		// TODO: Remove any backticks that don't have closing backticks.
		public string NormalizeMessage()
		{
			throw new NotImplementedException();
		}
	}
}
