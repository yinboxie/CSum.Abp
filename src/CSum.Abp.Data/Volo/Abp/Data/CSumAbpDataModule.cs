using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Volo.Abp.Data.Enumer;
using Volo.Abp.Modularity;

namespace Volo.Abp.Data
{
    public class CSumAbpDataModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

            var accessor = new EnumerAccessor();
            context.Services.AddSingleton<IEnumerAccessor>(_ => accessor);
            context.Services.AddSingleton<EnumerAccessor>(_ => accessor);
        }

        public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
        {
            CreateEnums(context.ServiceProvider);
        }

        private void CreateEnums(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var options = scope.ServiceProvider.GetRequiredService<IOptions<AbpEnumerOptions>>().Value;

                void ConfigureAll(IAbpEnumerConfigurationContext ctx, EnumerAccessor accessor)
                {
                    foreach (var configurator in options.Configurators)
                    {
                        configurator(ctx);
                    }

                    //移除忽略的枚举项
                    foreach (var item in ctx.Enums)
                    {
                        if (options.IgnoreEnums.Contains(item.Key))
                        {
                            ctx.Enums.Remove(item.Key);
                            ctx.EnumTypes.Remove(item.Key);
                        }
                    }
                    accessor.Enumer = ctx.Enums;
                    accessor.AllEnumType = ctx.EnumTypes;
                }

                ConfigureAll(new AbpEnumerConfigurationContext(new Dictionary<string, Dictionary<int, string>>(), scope.ServiceProvider), scope.ServiceProvider.GetRequiredService<EnumerAccessor>());
            }
        }
    }
}
