using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using CourseFeedbackSystem.Authorization;
using CourseFeedbackSystem.Courses.Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace CourseFeedbackSystem.Courses;

[AbpAuthorize(PermissionNames.Pages_Courses)]
public class CourseAppService : AsyncCrudAppService<Course, CourseDto, int, PagedCourseResultRequestDto, CourseDto, CourseDto>, ICourseAppService
{
    public CourseAppService(IRepository<Course, int> repository)
        : base(repository)
    {
    }

    protected override IQueryable<Course> CreateFilteredQuery(PagedCourseResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => 
                x.CourseName.Contains(input.Keyword) || 
                (x.InstructorName != null && x.InstructorName.Contains(input.Keyword)))
            .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);
    }

    protected override IQueryable<Course> ApplySorting(IQueryable<Course> query, PagedCourseResultRequestDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Sorting))
        {
            input.Sorting = "CourseName";
        }
        return base.ApplySorting(query, input);
    }

    public override async Task<CourseDto> CreateAsync(CourseDto input)
    {
        CheckCreatePermission();
        
        if (string.IsNullOrWhiteSpace(input.CourseName))
        {
            throw new Abp.UI.UserFriendlyException("Course name is required.");
        }

        var course = ObjectMapper.Map<Course>(input);
        await Repository.InsertAsync(course);
        await CurrentUnitOfWork.SaveChangesAsync();

        return MapToEntityDto(course);
    }

    public override async Task<CourseDto> UpdateAsync(CourseDto input)
    {
        CheckUpdatePermission();
        
        if (string.IsNullOrWhiteSpace(input.CourseName))
        {
            throw new Abp.UI.UserFriendlyException("Course name is required.");
        }

        var course = await Repository.GetAsync(input.Id);
        ObjectMapper.Map(input, course);
        await Repository.UpdateAsync(course);
        await CurrentUnitOfWork.SaveChangesAsync();

        return MapToEntityDto(course);
    }
}

