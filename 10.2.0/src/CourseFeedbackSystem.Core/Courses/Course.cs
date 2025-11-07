using Abp.Domain.Entities;

namespace CourseFeedbackSystem.Courses;

public class Course : Entity<int>
{
    public string CourseName { get; set; }
    public string InstructorName { get; set; }
    public bool IsActive { get; set; }
}

