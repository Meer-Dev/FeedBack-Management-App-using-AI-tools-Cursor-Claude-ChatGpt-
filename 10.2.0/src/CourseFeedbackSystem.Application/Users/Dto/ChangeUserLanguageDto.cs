using System.ComponentModel.DataAnnotations;

namespace CourseFeedbackSystem.Users.Dto;

public class ChangeUserLanguageDto
{
    [Required]
    public string LanguageName { get; set; }
}