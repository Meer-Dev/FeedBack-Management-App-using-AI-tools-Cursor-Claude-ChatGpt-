using Abp.Application.Services;
using CourseFeedbackSystem.MultiTenancy.Dto;

namespace CourseFeedbackSystem.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

