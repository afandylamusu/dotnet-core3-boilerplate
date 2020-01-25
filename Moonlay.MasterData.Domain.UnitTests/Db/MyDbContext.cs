using Microsoft.EntityFrameworkCore;
using Moonlay.Core.Models;
using Moonlay.MasterData.Domain.Customers;

namespace Moonlay.MasterData.Domain.UnitTests
{
    internal class MyDbContext : MoonlayDbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options, IDbTrailContext trailContext, ISignInService signInService) : base(options, trailContext, signInService)
        {
        }

        public DbSet<Customer> Customers => Set<Customer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(etb =>
            {
                etb.DefaultEntity();
            });
        }
    }

    internal class MyDbTrailContext : MoonlayDbTrailContext
    {
        public MyDbTrailContext(DbContextOptions<MyDbTrailContext> options) : base(options)
        {
        }

        public DbSet<CustomerTrail> CustomerTrails => Set<CustomerTrail>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerTrail>(etb =>
            {
                etb.HasKey(k => k.Id);
            });
        }
    }
}