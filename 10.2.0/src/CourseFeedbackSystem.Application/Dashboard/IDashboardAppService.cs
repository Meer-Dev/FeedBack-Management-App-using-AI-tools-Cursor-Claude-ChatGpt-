using Abp.Application.Services;
using CourseFeedbackSystem.Dashboard.Dto;
using System.Threading.Tasks;

namespace CourseFeedbackSystem.Dashboard;

public interface IDashboardAppService : IApplicationService
{
    Task<DashboardStatisticsDto> GetStatisticsAsync();
    Task<TopCoursesDto[]> GetTopCoursesByRatingAsync(int count = 5);
}

