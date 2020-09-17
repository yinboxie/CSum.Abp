using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Modularity;

namespace Volo.Abp.EntityFrameworkCore.SqlServer
{
    [DependsOn(typeof(CSumAbpEntityFrameworkCoreModule))]
    [DependsOn(typeof(AbpEntityFrameworkCoreSqlServerModule))]
    public class CSumAbpEntityFrameworkCoreSqlServerModule : AbpModule
    {
    }
}
