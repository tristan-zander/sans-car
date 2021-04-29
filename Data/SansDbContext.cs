using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data
{
    public class SansDbContext : DbContext
    {
        private string ConnectionString { get; }
        
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SongQueue> SongQueues { get; set; }
        public DbSet<Quote> Quotes { get; set; }

        public SansDbContext()
        {
            ConnectionString = ProjectConfiguration.ReadConfigFile().Database.ConnectionString;
        }
        
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