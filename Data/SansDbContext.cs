using System;
using System.Collections.Generic;
using System.Linq;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasPostgresExtension("fuzzystrmatch");
            builder.HasPostgresExtension("pg_trgm");

            builder
                .Entity<Quote>()
                .Property(q => q.Mentions)
                .HasConversion(
                    q => q.Select(Convert.ToDecimal).ToArray(),
                    q => q.Select(Convert.ToUInt64).ToHashSet()
                );

            builder
                .Entity<Quote>()
                .Navigation(q => q.Guild)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            builder
                .Entity<Quote>()
                .HasOne<Guild>()
                .WithMany(g => g.Quotes)
                .HasForeignKey(q => q.GuildId);

            builder
                .Entity<Guild>()
                .HasMany(g => g.Quotes)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
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
