using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using CourseFeedbackSystem.EntityFrameworkCore;
using CourseFeedbackSystem.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace CourseFeedbackSystem.Web.Tests;

[DependsOn(
    typeof(CourseFeedbackSystemWebMvcModule),
    typeof(AbpAspNetCoreTestBaseModule)
)]
public class CourseFeedbackSystemWebTestModule : AbpModule
{
    public CourseFeedbackSystemWebTestModule(CourseFeedbackSystemEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
    }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(CourseFeedbackSystemWebTestModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<ApplicationPartManager>()
            .AddApplicationPartsIfNotAddedBefore(typeof(CourseFeedbackSystemWebMvcModule).Assembly);
    }
}