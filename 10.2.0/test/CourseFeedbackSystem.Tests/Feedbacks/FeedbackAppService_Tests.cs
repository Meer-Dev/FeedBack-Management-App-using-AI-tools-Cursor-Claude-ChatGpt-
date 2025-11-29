using CourseFeedbackSystem.Courses;
using CourseFeedbackSystem.Feedbacks;
using CourseFeedbackSystem.Feedbacks.Dto;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace CourseFeedbackSystem.Tests.Feedbacks
{
    public class FeedbackAppService_Tests : CourseFeedbackSystemTestBase
    {
        private readonly IFeedbackAppService _feedbackAppService;

        public FeedbackAppService_Tests()
        {
            _feedbackAppService = Resolve<IFeedbackAppService>();
        }

        [Fact]
        public async Task Should_Create_Feedback_As_Host()
        {
            // Arrange
            LoginAsHostAdmin();

            var courseId = await UsingDbContextAsync(async context =>
            {
                var course = new Course
                {
                    CourseName = "Test Course",
                    InstructorName = "Test Instructor",
                    IsActive = true
                };
                await context.Courses.AddAsync(course);
                await context.SaveChangesAsync();
                return course.Id;
            });

            var input = new CreateUpdateFeedbackDto
            {
                CourseId = courseId,
                StudentName = "Test Student",
                Rating = 5,
                Comment = "Great course!"
            };

            // Act
            var output = await _feedbackAppService.CreateAsync(input);

            // Assert
            output.ShouldNotBeNull();
            output.StudentName.ShouldBe("Test Student");
        }
    }
}
