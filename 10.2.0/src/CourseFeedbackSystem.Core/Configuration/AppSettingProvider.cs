using Abp.Configuration;
using System.Collections.Generic;

namespace CourseFeedbackSystem.Configuration;

public class AppSettingProvider : SettingProvider
{
    public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
    {
        return new[]
        {
            new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()),
            new SettingDefinition(AppSettingNames.MaxFeedbackPerCourse, "100", scopes: SettingScopes.Tenant, clientVisibilityProvider: new VisibleSettingClientVisibilityProvider())
        };
    }
}
