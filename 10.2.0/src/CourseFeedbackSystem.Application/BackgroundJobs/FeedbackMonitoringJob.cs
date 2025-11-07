using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Logging;
using Castle.Core.Logging;
using CourseFeedbackSystem.Courses;
using CourseFeedbackSystem.Feedbacks;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CourseFeedbackSystem.BackgroundJobs;

public class FeedbackMonitoringJob : AsyncBackgroundJob<FeedbackMonitoringJobArgs>, ITransientDependency
{
    public new ILogger Logger { get; set; }
    
    private readonly IRepository<Course, int> _courseRepository;
    private readonly IRepository<Feedback, int> _feedbackRepository;

    public FeedbackMonitoringJob(
        IRepository<Course, int> courseRepository,
        IRepository<Feedback, int> feedbackRepository)
    {
        _courseRepository = courseRepository;
        _feedbackRepository = feedbackRepository;
        Logger = NullLogger.Instance; // Will be injected by ABP
    }

    public override async Task ExecuteAsync(FeedbackMonitoringJobArgs args)
    {
        try
        {
            var tenDaysAgo = DateTime.UtcNow.AddDays(-10);
            
            // Get all active courses
            var activeCourses = await _courseRepository.GetAllListAsync(c => c.IsActive);

            foreach (var course in activeCourses)
            {
                // Check if there are any feedbacks in the last 10 days
                var recentFeedbacks = await _feedbackRepository.GetAllListAsync(
                    f => f.CourseId == course.Id && f.CreatedDate >= tenDaysAgo);

                if (recentFeedbacks.Count == 0)
                {
                    var message = $"No feedback received for Course [{course.CourseName}] in last 10 days.";
                    Logger.Warn(message);
                    
                    // TODO: Optional - send notification email to admin
                    // await _emailSender.SendAsync(...);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Error in FeedbackMonitoringJob", ex);
            throw;
        }
    }
}

public class FeedbackMonitoringJobArgs
{
    // No arguments needed for this job
}

