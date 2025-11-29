using CourseFeedbackSystem.Courses;
using CourseFeedbackSystem.Feedbacks;
using CourseFeedbackSystem.Feedbacks.Dto;
using CourseFeedbackSystem.MultiTenancy;
using CourseFeedbackSystem.MultiTenancy.Dto;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CourseFeedbackSystem.Tests.Feedbacks
{
    public class ReproduceIssueTests : CourseFeedbackSystemTestBase
    {
        private readonly ITenantAppService _tenantAppService;
        private readonly IFeedbackAppService _feedbackAppService;

        public ReproduceIssueTests()
        {
            _tenantAppService = Resolve<ITenantAppService>();
            _feedbackAppService = Resolve<IFeedbackAppService>();
        }

        [Fact]
        public async Task Should_Create_Tenant_And_Add_Feedback()
        {
            // 1. Login as Host to create a tenant
            LoginAsHostAdmin();

            var tenancyName = "TestTenant" + Guid.NewGuid().ToString("N").Substring(0, 8);
            var tenantInput = new CreateTenantDto
            {
                TenancyName = tenancyName,
                Name = "Test Tenant",
                AdminEmailAddress = "admin@testtenant.com",
                IsActive = true
            };

            // Act: Create Tenant
            var tenantDto = await _tenantAppService.CreateAsync(tenantInput);
            tenantDto.ShouldNotBeNull();

            // 2. Login as the new Tenant
            LoginAsTenant(tenancyName, "admin");

            // 3. Create a Course for this tenant
            var courseId = await UsingDbContextAsync(async context =>
            {
                var course = new Course
                {
                    TenantId = tenantDto.Id,
                    CourseName = "Tenant Course",
                    InstructorName = "Tenant Instructor",
                    IsActive = true
                };
                await context.Courses.AddAsync(course);
                await context.SaveChangesAsync();
                return course.Id;
            });

            // 4. Add Feedback
            var feedbackInput = new CreateUpdateFeedbackDto
            {
                CourseId = courseId,
                StudentName = "Tenant Student",
                Rating = 5,
                Comment = "Great tenant course!"
            };

            var feedbackDto = await _feedbackAppService.CreateAsync(feedbackInput);

            // Assert
            feedbackDto.ShouldNotBeNull();
            feedbackDto.StudentName.ShouldBe("Tenant Student");
        }
    }
}
