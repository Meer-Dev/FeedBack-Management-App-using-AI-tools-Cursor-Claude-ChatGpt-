using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace CourseFeedbackSystem.Localization;

public static class CourseFeedbackSystemLocalizationConfigurer
{
    public static void Configure(ILocalizationConfiguration localizationConfiguration)
    {
        localizationConfiguration.Sources.Add(
            new DictionaryBasedLocalizationSource(CourseFeedbackSystemConsts.LocalizationSourceName,
                new XmlEmbeddedFileLocalizationDictionaryProvider(
                    typeof(CourseFeedbackSystemLocalizationConfigurer).GetAssembly(),
                    "CourseFeedbackSystem.Localization.SourceFiles"
                )
            )
        );
    }
}
