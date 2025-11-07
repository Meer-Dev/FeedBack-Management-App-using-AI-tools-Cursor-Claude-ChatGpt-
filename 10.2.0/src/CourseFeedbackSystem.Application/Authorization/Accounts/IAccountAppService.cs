using Abp.Application.Services;
using CourseFeedbackSystem.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace CourseFeedbackSystem.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
