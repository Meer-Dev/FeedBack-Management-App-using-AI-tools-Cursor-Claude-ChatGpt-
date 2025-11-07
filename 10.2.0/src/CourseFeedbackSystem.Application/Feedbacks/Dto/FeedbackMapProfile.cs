using AutoMapper;
using CourseFeedbackSystem.Feedbacks;

namespace CourseFeedbackSystem.Feedbacks.Dto
{
    public class FeedbackMapProfile : Profile
    {
        public FeedbackMapProfile()
        {
            // Map CreateUpdateFeedbackDto to Feedback (for Create/Update)
            CreateMap<CreateUpdateFeedbackDto, Feedback>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore());

            // Map Feedback to FeedbackDto (for Get)
            CreateMap<Feedback, FeedbackDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.CourseName : null));
            
            // Map FeedbackDto back to Feedback (if needed)
            CreateMap<FeedbackDto, Feedback>()
                .ForMember(dest => dest.Course, opt => opt.Ignore());
        }
    }
}