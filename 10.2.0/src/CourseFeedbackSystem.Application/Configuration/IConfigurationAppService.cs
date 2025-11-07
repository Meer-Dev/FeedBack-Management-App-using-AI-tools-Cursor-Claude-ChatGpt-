using CourseFeedbackSystem.Configuration.Dto;
using System.Threading.Tasks;

namespace CourseFeedbackSystem.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
