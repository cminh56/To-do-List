using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NoteBook_API.Models
{
    public partial class PRN231_NoteContext : DbContext
    {
        public PRN231_NoteContext()
        {
        }

        public PRN231_NoteContext(DbContextOptions<PRN231_NoteContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Note> Notes { get; set; } = null!;
        public virtual DbSet<Task> Tasks { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                                            .SetBasePath(Directory.GetCurrentDirectory())
                                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DB"));

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Note>(entity =>
            {
                entity.Property(e => e.Content).HasColumnType("nvarchar(max)").IsUnicode();
                entity.Property(e => e.Status).HasColumnType("nvarchar(50)").IsUnicode();
                entity.Property(e => e.Title)
                    .HasMaxLength(255).IsUnicode();

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notes)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Notes__UserId__3C69FB99");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.Property(e => e.Description).HasColumnType("nvarchar(max)").IsUnicode();

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.Priority)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode();

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Tasks__UserId__398D8EEE");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email, "UQ__Users__A9D1053443AA618F")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
