using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace CourseFeedbackSystem.EntityFrameworkCore;

public static class CourseFeedbackSystemDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<CourseFeedbackSystemDbContext> builder, string connectionString)
    {
        builder.UseSqlServer(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<CourseFeedbackSystemDbContext> builder, DbConnection connection)
    {
        builder.UseSqlServer(connection);
    }
}
