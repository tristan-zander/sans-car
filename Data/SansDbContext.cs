using System;
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
        public DbSet<Quote> QuotesToPublish { get; set; }

        public SansDbContext()
        {
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
