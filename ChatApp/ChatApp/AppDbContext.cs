using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace ChatApp
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Sender)
                .WithMany()
                .HasForeignKey("SenderId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Receiver)
                .WithMany()
                .HasForeignKey("ReceiverId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
