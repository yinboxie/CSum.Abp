using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AbpEfCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddCSumSqlServerRepositroy<TDbContext>(
            this IServiceCollection services)
            where TDbContext : AbpDbContext<TDbContext>
        {
            //注册自定义的默认仓储
            foreach (var entityType in CSumDbContextHelper.GetEntityTypes(typeof(TDbContext)))
            {
                //为聚合根实体注册默认仓储
                if (!typeof(IAggregateRoot).IsAssignableFrom(entityType))
                {
                    continue;
                }

                services.AddDefaultRepository(
                entityType,
                GetDefaultRepositoryImplementationType(typeof(TDbContext), entityType)
                );
            }

            return services;
        }

        private static Type GetDefaultRepositoryImplementationType(Type dbContextType, Type entityType)
        {
            var primaryKeyType = EntityHelper.FindPrimaryKeyType(entityType);

            if (primaryKeyType == null)
            {
                return typeof(CSumEfCoreSqlServerRepository<,>).MakeGenericType(dbContextType, entityType);
            }

            return typeof(CSumEfCoreSqlServerRepository<,,>).MakeGenericType(dbContextType, entityType, primaryKeyType);
        }
    }
}
