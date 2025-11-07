using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using CourseFeedbackSystem.Configuration;
using CourseFeedbackSystem.EntityFrameworkCore;
using CourseFeedbackSystem.Migrator.DependencyInjection;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace CourseFeedbackSystem.Migrator;

[DependsOn(typeof(CourseFeedbackSystemEntityFrameworkModule))]
public class CourseFeedbackSystemMigratorModule : AbpModule
{
    private readonly IConfigurationRoot _appConfiguration;

    public CourseFeedbackSystemMigratorModule(CourseFeedbackSystemEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

        _appConfiguration = AppConfigurations.Get(
            typeof(CourseFeedbackSystemMigratorModule).GetAssembly().GetDirectoryPathOrNull()
        );
    }

    public override void PreInitialize()
    {
        Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
            CourseFeedbackSystemConsts.ConnectionStringName
        );

        Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        Configuration.ReplaceService(
            typeof(IEventBus),
            () => IocManager.IocContainer.Register(
                Component.For<IEventBus>().Instance(NullEventBus.Instance)
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(CourseFeedbackSystemMigratorModule).GetAssembly());
        ServiceCollectionRegistrar.Register(IocManager);
    }
}
