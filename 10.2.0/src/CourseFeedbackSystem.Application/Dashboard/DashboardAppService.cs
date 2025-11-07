using Abp.Application.Services;
using Abp.Domain.Repositories;
using CourseFeedbackSystem.Courses;
using CourseFeedbackSystem.Dashboard.Dto;
using CourseFeedbackSystem.Feedbacks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CourseFeedbackSystem.Dashboard;

public class DashboardAppService : ApplicationService, IDashboardAppService
{
    private readonly IRepository<Course, int> _courseRepository;
    private readonly IRepository<Feedback, int> _feedbackRepository;

    public DashboardAppService(
        IRepository<Course, int> courseRepository,
        IRepository<Feedback, int> feedbackRepository)
    {
        _courseRepository = courseRepository;
        _feedbackRepository = feedbackRepository;
    }

    public async Task<DashboardStatisticsDto> GetStatisticsAsync()
    {
        var totalFeedbacks = await _feedbackRepository.CountAsync();
        var totalCourses = await _courseRepository.CountAsync();
        var activeCourses = await _courseRepository.CountAsync(c => c.IsActive);
        
        var averageRating = await _feedbackRepository.GetAll()
            .Select(f => (double?)f.Rating)
            .AverageAsync() ?? 0.0;

        return new DashboardStatisticsDto
        {
            TotalFeedbacks = totalFeedbacks,
            TotalCourses = totalCourses,
            ActiveCourses = activeCourses,
            AverageRating = averageRating
        };
    }

    public async Task<TopCoursesDto[]> GetTopCoursesByRatingAsync(int count = 5)
    {
        var topCourses = await (from course in _courseRepository.GetAll()
                                 join feedback in _feedbackRepository.GetAll() on course.Id equals feedback.CourseId into feedbacks
                                 where feedbacks.Any()
                                 select new TopCoursesDto
                                 {
                                     CourseId = course.Id,
                                     CourseName = course.CourseName,
                                     InstructorName = course.InstructorName,
                                     AverageRating = feedbacks.Average(f => (double)f.Rating),
                                     FeedbackCount = feedbacks.Count()
                                 })
                                 .OrderByDescending(x => x.AverageRating)
                                 .Take(count)
                                 .ToArrayAsync();

        return topCourses;
    }
}

