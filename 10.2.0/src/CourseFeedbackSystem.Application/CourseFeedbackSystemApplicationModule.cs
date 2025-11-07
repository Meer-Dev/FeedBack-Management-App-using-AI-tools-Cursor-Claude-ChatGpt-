using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using CourseFeedbackSystem.Authorization;

namespace CourseFeedbackSystem;

[DependsOn(
    typeof(CourseFeedbackSystemCoreModule),
    typeof(AbpAutoMapperModule))]
public class CourseFeedbackSystemApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Authorization.Providers.Add<CourseFeedbackSystemAuthorizationProvider>();
    }

    public override void Initialize()
    {
        var thisAssembly = typeof(CourseFeedbackSystemApplicationModule).GetAssembly();

        IocManager.RegisterAssemblyByConvention(thisAssembly);

        Configuration.Modules.AbpAutoMapper().Configurators.Add(
            // Scan the assembly for classes which inherit from AutoMapper.Profile
            cfg => cfg.AddMaps(thisAssembly)
        );
    }
}
