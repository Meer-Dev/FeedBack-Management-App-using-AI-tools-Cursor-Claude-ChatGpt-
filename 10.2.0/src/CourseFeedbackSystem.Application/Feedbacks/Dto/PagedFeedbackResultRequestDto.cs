using Abp.Application.Services.Dto;

namespace CourseFeedbackSystem.Feedbacks.Dto;

public class PagedFeedbackResultRequestDto : PagedAndSortedResultRequestDto
{
    public string Keyword { get; set; }
    public int? CourseId { get; set; }
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }
}

