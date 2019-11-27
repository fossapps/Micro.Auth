using Micro.Auth.Api.Configs;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Micro.Auth.Api.Models
{
    public class ApplicationContext : IdentityDbContext
    {
        private readonly DatabaseConfig _db;
        public new DbSet<User> Users { set; get; }
        public DbSet<RefreshToken> RefreshTokens { set; get; }

        public ApplicationContext(DbContextOptions options, IOptions<DatabaseConfig> dbOption) : base(options)
        {
            _db = dbOption.Value;
        }

        protected ApplicationContext(IOptions<DatabaseConfig> dbOption)
        {
            _db = dbOption.Value;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = new NpgsqlConnectionStringBuilder
            {
                Host = _db.Host,
                Port = _db.Port,
                Database = _db.Name,
                Username = _db.User,
                Password = _db.Password,
                SslMode = SslMode.Disable
            };
            optionsBuilder.UseNpgsql(connection.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
