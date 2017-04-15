using Microsoft.EntityFrameworkCore;
using Warbler.Infrastructure.Chat.Models;

namespace Warbler.Areas.Chat.Data
{
    /// <summary>
    ///   Coordinates Entity Framework functionality.
    /// </summary>
    public class ChatContext : DbContext
    {
        public ChatContext(DbContextOptions<ChatContext> options)
            : base(options)
        { }

        public DbSet<University> Universities { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Membership> Memberships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<University>()
                .ToTable(nameof(University))
                .HasOne(u => u.Server)
                .WithOne(s => s.University)
                .HasForeignKey<Server>(s => s.UniversityId);

            modelBuilder.Entity<Server>().ToTable(nameof(Server));
            modelBuilder.Entity<Channel>().ToTable(nameof(Channel));
            modelBuilder.Entity<User>().ToTable(nameof(User));
            modelBuilder.Entity<Message>().ToTable(nameof(Message));
            modelBuilder.Entity<Membership>().ToTable(nameof(Membership));
        }
    }
}
