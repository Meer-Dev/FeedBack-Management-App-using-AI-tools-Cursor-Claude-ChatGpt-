using Abp.Domain.Entities;
using CourseFeedbackSystem.Courses;
using System;

namespace CourseFeedbackSystem.Feedbacks;

public class Feedback : Entity<int>
{
    public string StudentName { get; set; }
    public int CourseId { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; } // 1-5
    public DateTime CreatedDate { get; set; }
    public string FileUrl { get; set; }
    
    // Navigation property
    public virtual Course Course { get; set; }
}

