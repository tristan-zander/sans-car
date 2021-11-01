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

        public SansDbContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(ConnectionString);
            }
        }
    }
}