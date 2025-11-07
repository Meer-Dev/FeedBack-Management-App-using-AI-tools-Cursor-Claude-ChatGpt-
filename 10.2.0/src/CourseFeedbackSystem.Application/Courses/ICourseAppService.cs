using Abp.Application.Services;
using CourseFeedbackSystem.Courses.Dto;

namespace CourseFeedbackSystem.Courses;

public interface ICourseAppService : IAsyncCrudAppService<CourseDto, int, PagedCourseResultRequestDto, CourseDto, CourseDto>
{
}

