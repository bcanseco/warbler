using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Warbler.Models;

namespace Warbler.Misc
{
    public class WarblerDbContext : IdentityDbContext<User>
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        public WarblerDbContext(DbContextOptions<WarblerDbContext> options)
            : base(options)
        { }

        // DbSet<Users> is generated in base class
        public DbSet<University> Universities { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Membership> Memberships { get; set; }
//<<<<<<< HEAD
        //public DbSet<ClaimsRequest> ClaimsRequests { get; set; }
        //Public DbSet<BlockedUsers> BlockedUsers { get; set; }
        public DbSet<ChannelTemplate> ChannelTemplates { get; set; }
//=======
        public DbSet<ClaimRequest> ClaimRequests { get; set; }
//>>>>>>> a70517091a104e7239ce0de04872245e0d41e96a

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
//<<<<<<< HEAD
            //modelBuilder.Entity<ClaimsRequest>.ToTable(nameof(ClaimsRequest));
            //modelBuilder.Entity<BlockedUser>.ToTable(nameof(BlockedUser));
//=======
            modelBuilder.Entity<ClaimRequest>().ToTable(nameof(ClaimRequest));
            modelBuilder.Entity<ChannelTemplate>().ToTable(nameof(ChannelTemplate));
//>>>>>>> a70517091a104e7239ce0de04872245e0d41e96a
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
