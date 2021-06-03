using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Micro.Auth.Storage
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
            optionsBuilder.UseNpgsql(connection.ConnectionString, options =>
            {
                options.MigrationsAssembly("Micro.Auth.Storage");
            });
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var roles = new List<IdentityRole>
            {
                new IdentityRole("service_account") {Id = "9a6eb015-82d1-480c-b962-5aab596ef4f6", NormalizedName = "SERVICE_ACCOUNT"}
            };
            builder.Entity<IdentityRole>().HasData(roles);
            var claims =
            new List<IdentityRoleClaim<string>>{
                new IdentityRoleClaim<string>()
                {
                    Id = 1,
                    RoleId = roles[0].Id,
                    ClaimType = "token_expiry",
                    ClaimValue = "1440",
                },
            };
            builder.Entity<IdentityRoleClaim<string>>().HasData(claims);
        }
    }
}