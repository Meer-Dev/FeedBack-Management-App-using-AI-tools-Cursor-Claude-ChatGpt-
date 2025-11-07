using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace CourseFeedbackSystem.Feedbacks.Dto;

public class CreateUpdateFeedbackDto : IEntityDto<int>
{
    public int Id { get; set; } // 0 for create

    [Required]
    [StringLength(200)]
    public string StudentName { get; set; }

    [Required]
    public int CourseId { get; set; }

    public string Comment { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; } // 1-5

    public string FileUrl { get; set; } // optional
}

