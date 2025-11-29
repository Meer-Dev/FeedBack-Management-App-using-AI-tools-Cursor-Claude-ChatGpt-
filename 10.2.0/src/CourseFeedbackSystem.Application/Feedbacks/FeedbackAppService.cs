using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using CourseFeedbackSystem.Authorization;
using CourseFeedbackSystem.Configuration;
using CourseFeedbackSystem.Courses;
using CourseFeedbackSystem.Feedbacks.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace CourseFeedbackSystem.Feedbacks;

[AbpAuthorize(PermissionNames.Pages_Feedbacks)]
public class FeedbackAppService : AsyncCrudAppService<Feedback, FeedbackDto, int, PagedFeedbackResultRequestDto, CreateUpdateFeedbackDto, CreateUpdateFeedbackDto>, IFeedbackAppService
{
    private readonly IRepository<Course, int> _courseRepository;
    private readonly ISettingManager _settingManager;

    public FeedbackAppService(
        IRepository<Feedback, int> repository,
        IRepository<Course, int> courseRepository,
        ISettingManager settingManager)
        : base(repository)
    {
        _courseRepository = courseRepository;
        _settingManager = settingManager;
    }

    protected override IQueryable<Feedback> CreateFilteredQuery(PagedFeedbackResultRequestDto input)
    {
        return Repository.GetAllIncluding(x => x.Course)
            .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => 
                x.StudentName.Contains(input.Keyword) || 
                (x.Comment != null && x.Comment.Contains(input.Keyword)))
            .WhereIf(input.CourseId.HasValue, x => x.CourseId == input.CourseId.Value)
            .WhereIf(input.MinRating.HasValue, x => x.Rating >= input.MinRating.Value)
            .WhereIf(input.MaxRating.HasValue, x => x.Rating <= input.MaxRating.Value);
    }

    protected override IQueryable<Feedback> ApplySorting(IQueryable<Feedback> query, PagedFeedbackResultRequestDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Sorting))
        {
            input.Sorting = "CreatedDate DESC";
        }
        return base.ApplySorting(query, input);
    }

    protected override FeedbackDto MapToEntityDto(Feedback entity)
    {
        var dto = base.MapToEntityDto(entity);
        if (entity.Course != null)
        {
            dto.CourseName = entity.Course.CourseName;
        }
        return dto;
    }

    public override async Task<FeedbackDto> CreateAsync(CreateUpdateFeedbackDto input)
    {
        CheckCreatePermission();

        // Validate rating
        if (input.Rating < 1 || input.Rating > 5)
        {
            throw new UserFriendlyException("Rating must be between 1 and 5.");
        }

        // Validate course exists and is active
        var course = await _courseRepository.GetAsync(input.CourseId);
        if (course == null)
        {
            throw new UserFriendlyException("Course not found.");
        }

        if (!course.IsActive)
        {
            throw new UserFriendlyException("Cannot add feedback to an inactive course.");
        }

        // Check tenant setting for max feedbacks per course (only for tenant users)
        if (AbpSession.TenantId.HasValue)
        {
            var maxFeedbackPerCourse = await _settingManager.GetSettingValueForTenantAsync<int>(
                AppSettingNames.MaxFeedbackPerCourse, 
                AbpSession.TenantId.Value);

            if (maxFeedbackPerCourse > 0)
            {
                var feedbackCount = await Repository.CountAsync(x => x.CourseId == input.CourseId);
                if (feedbackCount >= maxFeedbackPerCourse)
                {
                    throw new UserFriendlyException($"Maximum feedback limit ({maxFeedbackPerCourse}) reached for this course.");
                }
            }
        }

        var feedback = ObjectMapper.Map<Feedback>(input);
        feedback.CreatedDate = DateTime.UtcNow;
        
        // Explicitly set TenantId for multi-tenancy
        if (AbpSession.TenantId.HasValue)
        {
            feedback.TenantId = AbpSession.TenantId.Value;
        }

        await Repository.InsertAsync(feedback);
        await CurrentUnitOfWork.SaveChangesAsync();

        return await GetAsync(new EntityDto<int> { Id = feedback.Id });
    }

    public override async Task<FeedbackDto> UpdateAsync(CreateUpdateFeedbackDto input)
    {
        CheckUpdatePermission();

        // Validate rating
        if (input.Rating < 1 || input.Rating > 5)
        {
            throw new UserFriendlyException("Rating must be between 1 and 5.");
        }

        // Validate course exists and is active
        var course = await _courseRepository.GetAsync(input.CourseId);
        if (course == null)
        {
            throw new UserFriendlyException("Course not found.");
        }

        var feedback = await Repository.GetAsync(input.Id);
        ObjectMapper.Map(input, feedback);

        await Repository.UpdateAsync(feedback);
        await CurrentUnitOfWork.SaveChangesAsync();

        return await GetAsync(new EntityDto<int> { Id = feedback.Id });
    }
}

