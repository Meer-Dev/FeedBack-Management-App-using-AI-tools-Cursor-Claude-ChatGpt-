using CourseFeedbackSystem.Configuration;
using CourseFeedbackSystem.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CourseFeedbackSystem.EntityFrameworkCore;

/* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
public class CourseFeedbackSystemDbContextFactory : IDesignTimeDbContextFactory<CourseFeedbackSystemDbContext>
{
    public CourseFeedbackSystemDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<CourseFeedbackSystemDbContext>();

        /*
         You can provide an environmentName parameter to the AppConfigurations.Get method. 
         In this case, AppConfigurations will try to read appsettings.{environmentName}.json.
         Use Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") method or from string[] args to get environment if necessary.
         https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#args
         */
        var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

        CourseFeedbackSystemDbContextConfigurer.Configure(builder, configuration.GetConnectionString(CourseFeedbackSystemConsts.ConnectionStringName));

        return new CourseFeedbackSystemDbContext(builder.Options);
    }
}
