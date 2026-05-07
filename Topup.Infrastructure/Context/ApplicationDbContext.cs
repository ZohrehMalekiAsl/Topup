using Microsoft.EntityFrameworkCore;
using Topup.Domain.Entities;
using Topup.Infrastructure.Configs;

namespace Topup.Infrastructure.Context
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<ChargeRequest> ChargeRequest { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ChargeRequest>();
            builder.ApplyConfigurationsFromAssembly(typeof(ChargeRequestConfig).Assembly);
        }
    }
}
