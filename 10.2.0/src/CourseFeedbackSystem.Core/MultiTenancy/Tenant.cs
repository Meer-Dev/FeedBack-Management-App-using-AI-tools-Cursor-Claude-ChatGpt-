using Abp.MultiTenancy;
using CourseFeedbackSystem.Authorization.Users;

namespace CourseFeedbackSystem.MultiTenancy;

public class Tenant : AbpTenant<User>
{
    public Tenant()
    {
    }

    public Tenant(string tenancyName, string name)
        : base(tenancyName, name)
    {
    }
}
