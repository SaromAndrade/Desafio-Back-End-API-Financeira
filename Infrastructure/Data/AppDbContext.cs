using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Definir índice único para a coluna Name da entidade User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Name)
                .IsUnique();

            // Configuração do relacionamento 1:1 entre User e Wallet
            modelBuilder.Entity<User>()
                .HasOne(u => u.Wallet)
                .WithOne(w => w.User)
                .HasForeignKey<Wallet>(w => w.UserId);

            // Configuração do relacionamento N:1 entre Transaction e Wallet
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId);

            // Configuração do relacionamento N:1 entre Transaction e SenderUser
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.SenderUser)
                .WithMany()
                .HasForeignKey(t => t.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict); // Evita exclusão em cascata

            // Configuração do relacionamento N:1 entre Transaction e ReceiverUser
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ReceiverUser)
                .WithMany()
                .HasForeignKey(t => t.ReceiverUserId)
                .OnDelete(DeleteBehavior.Restrict); // Evita exclusão em cascata
        }
    }
}
