using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TraineeCoreAPI.Models;

public partial class TraineeDbContext : DbContext
{
    public TraineeDbContext()
    {
    }

    public TraineeDbContext(DbContextOptions<TraineeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Trainee> Trainees { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Database=TraineeDB; Trusted_Connection=true; TrustServerCertificate=true; MultipleActiveResultSets=True; Integrated Security=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__C92D71A70725617F");

            entity.ToTable("Course");

            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Trainee).WithMany(p => p.Courses)
                .HasForeignKey(d => d.TraineeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Course__TraineeI__286302EC");
        });

        modelBuilder.Entity<Trainee>(entity =>
        {
            entity.HasKey(e => e.TraineeId).HasName("PK__Trainee__3BA911CAE2B05D80");

            entity.ToTable("Trainee");

            entity.Property(e => e.BirhDate).HasColumnType("datetime");
            entity.Property(e => e.ImageName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TraineeName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C5716FFC8");

            entity.ToTable("User");

            entity.Property(e => e.EmailId).IsUnicode(false);
            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
