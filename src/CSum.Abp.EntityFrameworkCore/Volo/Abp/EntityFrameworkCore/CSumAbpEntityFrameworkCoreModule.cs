using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Volo.Abp.EntityFrameworkCore
{
    [DependsOn(typeof(AbpEntityFrameworkCoreModule))]
    [DependsOn(typeof(CSumAbpDddDomainModule))]
    public class CSumAbpEntityFrameworkCoreModule : AbpModule
    {
    }
}
