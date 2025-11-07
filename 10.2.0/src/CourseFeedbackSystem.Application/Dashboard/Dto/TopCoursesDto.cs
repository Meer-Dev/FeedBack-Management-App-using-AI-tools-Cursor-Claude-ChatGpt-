namespace CourseFeedbackSystem.Dashboard.Dto;

public class TopCoursesDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public string InstructorName { get; set; }
    public double AverageRating { get; set; }
    public int FeedbackCount { get; set; }
}

