using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CourseFeedbackSystem.Courses;
using System.ComponentModel.DataAnnotations;

namespace CourseFeedbackSystem.Courses.Dto;

[AutoMapFrom(typeof(Course))]
[AutoMapTo(typeof(Course))]
public class CourseDto : EntityDto<int>
{
    [Required]
    [StringLength(250)]
    public string CourseName { get; set; }

    [StringLength(250)]
    public string InstructorName { get; set; }

    public bool IsActive { get; set; }
}

