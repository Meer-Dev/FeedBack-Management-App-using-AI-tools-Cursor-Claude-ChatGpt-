using Abp.Zero.EntityFrameworkCore;
using CourseFeedbackSystem.Authorization.Roles;
using CourseFeedbackSystem.Authorization.Users;
using CourseFeedbackSystem.Courses;
using CourseFeedbackSystem.Feedbacks;
using CourseFeedbackSystem.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace CourseFeedbackSystem.EntityFrameworkCore;

public class CourseFeedbackSystemDbContext : AbpZeroDbContext<Tenant, Role, User, CourseFeedbackSystemDbContext>
{
    /* Define a DbSet for each entity of the application */
    public DbSet<Course> Courses { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }

    public CourseFeedbackSystemDbContext(DbContextOptions<CourseFeedbackSystemDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Course configuration
        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.CourseName).IsRequired().HasMaxLength(250);
            entity.Property(e => e.InstructorName).HasMaxLength(250);
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        });

        // Feedback configuration
        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.ToTable("Feedbacks");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.StudentName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Comment).HasColumnType("NVARCHAR(MAX)");
            entity.Property(e => e.Rating).IsRequired();
            entity.Property(e => e.CreatedDate).IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(e => e.FileUrl).HasMaxLength(500);
            
            entity.ToTable(t => t.HasCheckConstraint("CK_Feedbacks_Rating", "[Rating] BETWEEN 1 AND 5"));
            
            entity.HasOne(e => e.Course)
                .WithMany()
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
