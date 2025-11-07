using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using CourseFeedbackSystem.EntityFrameworkCore.Seed;

namespace CourseFeedbackSystem.EntityFrameworkCore;

[DependsOn(
    typeof(CourseFeedbackSystemCoreModule),
    typeof(AbpZeroCoreEntityFrameworkCoreModule))]
public class CourseFeedbackSystemEntityFrameworkModule : AbpModule
{
    /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
    public bool SkipDbContextRegistration { get; set; }

    public bool SkipDbSeed { get; set; }

    public override void PreInitialize()
    {
        if (!SkipDbContextRegistration)
        {
            Configuration.Modules.AbpEfCore().AddDbContext<CourseFeedbackSystemDbContext>(options =>
            {
                if (options.ExistingConnection != null)
                {
                    CourseFeedbackSystemDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                }
                else
                {
                    CourseFeedbackSystemDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                }
            });
        }
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(CourseFeedbackSystemEntityFrameworkModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        if (!SkipDbSeed)
        {
            SeedHelper.SeedHostDb(IocManager);
        }
    }
}
