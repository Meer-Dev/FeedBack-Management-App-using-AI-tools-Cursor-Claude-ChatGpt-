using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System;

namespace CourseFeedbackSystem.Web.Host.Startup
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            
            // Allow access if user is authenticated
            // In production, you might want to check for admin role
            return httpContext.User.Identity.IsAuthenticated;
        }
    }
}

