using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace Volo.Abp.Domain
{
    [DependsOn(typeof(CSumAbpDataModule))]
    [DependsOn(typeof(AbpDddDomainModule))]
    public class CSumAbpDddDomainModule : AbpModule
    {
    }
}
