using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class SansDbContext : IdentityDbContext
    {
        private string ConnectionString { get; }

        public DbSet<Guild> Guilds { get; set; }
        public DbSet<DiscordChannel> Channels { get; set; }
        public DbSet<Quote> Quotes { get; set; }

        public SansDbContext()
        {
            // TODO: remove this before 2.0
            ConnectionString = "Host=localhost;Database=sans_car;";
        }

        public SansDbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public SansDbContext(DbContextOptions<SansDbContext> options)
            : base(options)
        {
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
