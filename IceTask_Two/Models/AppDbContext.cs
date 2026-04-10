using IceTask_Two.Models;
using Microsoft.EntityFrameworkCore;

namespace ContractCentral.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients => Set<Client>();

        /// <summary>Matches existing repository/controller usage (<c>Contract_Info</c>).</summary>
        public DbSet<Contract> Contract_Info => Set<Contract>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany(c => c.Contracts)
                .HasForeignKey(c => c.ClientId);
        }
    }
}
