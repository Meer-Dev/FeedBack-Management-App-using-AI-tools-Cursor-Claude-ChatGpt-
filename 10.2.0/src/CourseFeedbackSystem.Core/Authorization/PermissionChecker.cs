using Abp.Authorization;
using CourseFeedbackSystem.Authorization.Roles;
using CourseFeedbackSystem.Authorization.Users;

namespace CourseFeedbackSystem.Authorization;

public class PermissionChecker : PermissionChecker<Role, User>
{
    public PermissionChecker(UserManager userManager)
        : base(userManager)
    {
    }
}
