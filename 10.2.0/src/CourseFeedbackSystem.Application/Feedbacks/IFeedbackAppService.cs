using Abp.Application.Services;
using CourseFeedbackSystem.Feedbacks.Dto;

namespace CourseFeedbackSystem.Feedbacks;

public interface IFeedbackAppService : IAsyncCrudAppService<FeedbackDto, int, PagedFeedbackResultRequestDto, CreateUpdateFeedbackDto, CreateUpdateFeedbackDto>
{
}

