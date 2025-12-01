using CourseFeedbackSystem.Courses;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CourseFeedbackSystem.EntityFrameworkCore.Seed.Tenants
{
    public class InitialCourseBuilder
    {
        private readonly CourseFeedbackSystemDbContext _context;
        private readonly int _tenantId;

        public InitialCourseBuilder(CourseFeedbackSystemDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            var course = _context.Courses.IgnoreQueryFilters().FirstOrDefault(c => c.TenantId == _tenantId && c.CourseName == "CS101 - Introduction to Programming");
            if (course == null)
            {
                _context.Courses.Add(new Course
                {
                    TenantId = _tenantId,
                    CourseName = "CS101 - Introduction to Programming",
                    InstructorName = "Dr. John Smith",
                    IsActive = true
                });
                _context.SaveChanges();
            }
        }
    }
}
