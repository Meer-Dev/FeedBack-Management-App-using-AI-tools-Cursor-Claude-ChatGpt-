using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace CourseFeedbackSystem.Feedbacks.Dto
{
    public class FeedbackDto : EntityDto<int>
    {
        [Required]
        [StringLength(200)]
        public string StudentName { get; set; }

        [Required]
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public string Comment { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime CreatedDate { get; set; }

        public string FileUrl { get; set; }
    }
}