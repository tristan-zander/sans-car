// Licensed under the Mozilla Public License 2.0.
// Usage of these files must be in agreement with the license.
//
// You may find a copy of the license at https://www.mozilla.org/en-US/MPL/2.0/

using Microsoft.EntityFrameworkCore;

namespace Data
{
	public class SansDbContext : DbContext
	{
		private string ConnectionString { get; }

		public DbSet<Guild> Guilds { get; set; }
		public DbSet<Channel> Channels { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Quote> Quotes { get; set; }

		public SansDbContext()
		{
			ConnectionString = "Host=localhost;Database=sans_car;";
		}

		public SansDbContext(string connectionString)
		{
			ConnectionString = connectionString;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured && ConnectionString != null)
			{
				optionsBuilder.UseNpgsql(ConnectionString);
			}
		}
	}
}
