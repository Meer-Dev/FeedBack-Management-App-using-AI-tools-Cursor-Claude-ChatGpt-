using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace CourseFeedbackSystem.Controllers
{
    public abstract class CourseFeedbackSystemControllerBase : AbpController
    {
        protected CourseFeedbackSystemControllerBase()
        {
            LocalizationSourceName = CourseFeedbackSystemConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
