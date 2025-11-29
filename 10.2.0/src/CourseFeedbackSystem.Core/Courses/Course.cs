using Abp.Domain.Entities;

namespace CourseFeedbackSystem.Courses;

public class Course : Entity<int>, IMustHaveTenant
{
    public int TenantId { get; set; }
    public string CourseName { get; set; }
    public string InstructorName { get; set; }
    public bool IsActive { get; set; }
}

