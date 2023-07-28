using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PwdGenDLL.Models;

using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace PwdGenDLL
{
    public class AppDbContext : DbContext
    {
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<Encryption> Encryptions { get; set; }
        public DbSet<PasswordHistory> PasswordHistories { get; set; }

        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=DBStorage/PwdGenDB.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Settings>()
                        .HasIndex(s => new { s.EncryptionId, s.KeyId })
                        .IsUnique();

            modelBuilder.Entity<Key>()
                        .HasIndex(k => k.Value)
                        .IsUnique();

            modelBuilder.Entity<Key>()
                        .Property(k => k.Value)
                        .IsRequired();

            modelBuilder.Entity<Key>()
                        .ToTable(t => t.HasCheckConstraint("CHK_Key_Value_MaxLength", "LENGTH(Value) <= 100"));

            modelBuilder.Entity<Encryption>()
                        .HasIndex(k => k.Name)
                        .IsUnique();

            modelBuilder.Entity<Encryption>()
                        .Property(k => k.Name)
                        .IsRequired();

            modelBuilder.Entity<Encryption>()
                        .ToTable(t => t.HasCheckConstraint("CHK_Encryption_Name_MaxLength", "LENGTH(Name) <= 100"));

            modelBuilder.Entity<PasswordHistory>()
                        .Property(ph => ph.EncryptedText)
                        .IsRequired();

            modelBuilder.Entity<PasswordHistory>()
                        .ToTable(t => t.HasCheckConstraint("CHK_PasswordHistory_SourceText_MaxLength", "LENGTH(SourceText) <= 100"));

            modelBuilder.Entity<Settings>()
                        .Property(ph => ph.EncryptionId)
                        .IsRequired();

            modelBuilder.Entity<Settings>()
                        .Property(ph => ph.KeyId)
                        .IsRequired();

            modelBuilder.Entity<Settings>()
                        .HasIndex(s => new { s.EncryptionId, s.KeyId })
                        .IsUnique();
        }
    }
}
