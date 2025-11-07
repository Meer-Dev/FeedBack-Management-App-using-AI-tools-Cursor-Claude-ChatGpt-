using Abp.Authorization;
using Abp.Runtime.Session;
using CourseFeedbackSystem.Configuration.Dto;
using System.Threading.Tasks;

namespace CourseFeedbackSystem.Configuration;

[AbpAuthorize]
public class ConfigurationAppService : CourseFeedbackSystemAppServiceBase, IConfigurationAppService
{
    public async Task ChangeUiTheme(ChangeUiThemeInput input)
    {
        await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
    }
}
