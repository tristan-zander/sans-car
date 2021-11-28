// Licensed under the Mozilla Public License 2.0.
// Usage of these files must be in agreement with the license.
//
// You may find a copy of the license at https://www.mozilla.org/en-US/MPL/2.0/

using System;
using System.ComponentModel.DataAnnotations;

namespace Data
{
	public class Song
	{
		[Key] public string SongId { get; set; }
		[Required]
		public string Title { get; set; }
		[Required]
		public string Author { get; set; }
		[Required]
		public TimeSpan Length { get; set; }
		[Required]
		public Uri Uri { get; set; }
	}
}
