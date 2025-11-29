using CourseFeedbackSystem.Courses;
using CourseFeedbackSystem.Feedbacks;
using CourseFeedbackSystem.Feedbacks.Dto;
using CourseFeedbackSystem.MultiTenancy;
using CourseFeedbackSystem.MultiTenancy.Dto;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;
using Abp.UI;
using Abp.Domain.Repositories;

namespace CourseFeedbackSystem.Tests.Feedbacks
{
    public class IsolationTests : CourseFeedbackSystemTestBase
    {
        private readonly ITenantAppService _tenantAppService;
        private readonly IFeedbackAppService _feedbackAppService;

        public IsolationTests()
        {
            _tenantAppService = Resolve<ITenantAppService>();
            _feedbackAppService = Resolve<IFeedbackAppService>();
        }

        [Fact]
        public async Task Tenant_Should_Not_See_Host_Course()
        {
            // 1. Login as Host and create a course
            LoginAsHostAdmin();
            var courseId = await UsingDbContextAsync(async context =>
            {
                var course = new Course
                {
                    TenantId = 1, // Assign to Default Tenant (or Host if we used null, but now it is int)
                                  // Wait, if it is IMustHaveTenant, TenantId is required.
                                  // Host usually sees all, but if we create it with TenantId=1, it belongs to Tenant 1.
                                  // Let's create it for a specific tenant A.
                    CourseName = "Host Created Course",
                    InstructorName = "Host Instructor",
                    IsActive = true
                };
                // We need to set TenantId explicitly if we are Host? 
                // Or if we are Host, we can create data for any tenant.
                // Let's say Host creates data for Default Tenant (1).
                course.TenantId = 1; 
                
                await context.Courses.AddAsync(course);
                await context.SaveChangesAsync();
                return course.Id;
            });

            // 2. Create Tenant B
            var tenantBName = "TenantB" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var tenantB = await _tenantAppService.CreateAsync(new CreateTenantDto
            {
                TenancyName = tenantBName,
                Name = "Tenant B",
                AdminEmailAddress = "admin@tenantb.com",
                IsActive = true
            });

            // 3. Login as Tenant B
            LoginAsTenant(tenantBName, "admin");

            // 4. Try to add feedback to the course (which belongs to Tenant 1)
            var feedbackInput = new CreateUpdateFeedbackDto
            {
                CourseId = courseId,
                StudentName = "Tenant B Student",
                Rating = 5,
                Comment = "I should not see this course!"
            };

            // Act & Assert
            // Should fail because Course is not found for Tenant B
            await Assert.ThrowsAsync<UserFriendlyException>(async () => 
            {
                await _feedbackAppService.CreateAsync(feedbackInput);
            });
        }
    }
}
