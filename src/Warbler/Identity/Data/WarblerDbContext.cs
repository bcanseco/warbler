using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Warbler.Areas.Chat.Models;

namespace Warbler.Identity.Data
{
    public class WarblerDbContext : IdentityDbContext<User>
    {
        public WarblerDbContext(DbContextOptions<WarblerDbContext> options)
            : base(options)
        { }

        // Users DbSet is generated in base class
        public DbSet<University> Universities { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Membership> Memberships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<University>()
                .ToTable(nameof(University))
                .HasOne(u => u.Server)
                .WithOne(s => s.University)
                .HasForeignKey<Server>(s => s.UniversityId);

            modelBuilder.Entity<Server>().ToTable(nameof(Server));
            modelBuilder.Entity<Channel>().ToTable(nameof(Channel));
            modelBuilder.Entity<Message>().ToTable(nameof(Message));
            modelBuilder.Entity<Membership>().ToTable(nameof(Membership));
        }
    }
}
