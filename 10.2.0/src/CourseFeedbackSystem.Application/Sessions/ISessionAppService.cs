using Abp.Application.Services;
using CourseFeedbackSystem.Sessions.Dto;
using System.Threading.Tasks;

namespace CourseFeedbackSystem.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
