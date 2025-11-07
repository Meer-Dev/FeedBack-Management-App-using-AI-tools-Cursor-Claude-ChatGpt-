using Abp.Application.Services.Dto;

namespace CourseFeedbackSystem.Courses.Dto;

public class PagedCourseResultRequestDto : PagedAndSortedResultRequestDto
{
    public string Keyword { get; set; }
    public bool? IsActive { get; set; }
}

