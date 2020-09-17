using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq;
using System.Linq.Expressions;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Volo.Abp.EntityFrameworkCore
{
    /// <summary>
    /// 基于<see cref="AbpDbContext<TDbContext>"/>上下文的扩展
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    public abstract class CSumAbpDbContext<TDbContext> : AbpDbContext<TDbContext>
        where TDbContext : DbContext
    {
        protected CSumAbpDbContext(DbContextOptions<TDbContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(() => CSumDbFunctions.ConvertToInt64(default))
                .HasTranslation(args => {
                    var argments = args.ToList();
                    argments.Insert(0, new SqlFragmentExpression("bigint")); // CONVERT需要两个参数
                    return SqlFunctionExpression.Create(
                        "CONVERT",
                        argments,
                        typeof(long),
                        null);
                }
                     
            );
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// 重写添加实体拦截
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="changeReport"></param>
        protected override void ApplyAbpConceptsForAddedEntity(EntityEntry entry, EntityChangeReport changeReport)
        {
            SetOrUpdateSampleSpelling(entry);
            base.ApplyAbpConceptsForAddedEntity(entry, changeReport);
        }

        /// <summary>
        /// 重写更新实体拦截
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="changeReport"></param>
        protected override void ApplyAbpConceptsForModifiedEntity(EntityEntry entry, EntityChangeReport changeReport)
        {
            SetOrUpdateSampleSpelling(entry);
            base.ApplyAbpConceptsForModifiedEntity(entry, changeReport);
        }

        /// <summary>
        /// 设置或更新助记码字段
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void SetOrUpdateSampleSpelling(EntityEntry entry)
        {
            var entity = entry.Entity as IHasSimpleSpelling;
            if (entity == null)
            {
                return;
            }

            entity.SimpleSpelling = Str.PinYin(entity.GetSimpleSpellingName());
        }

        /// <summary>
        /// 重写设置属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="mutableEntityType"></param>
        protected override void ConfigureBaseProperties<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
        {
            if (mutableEntityType.IsOwned())
            {
                return;
            }

            //关键代码，采用CSum扩展的配置约定
            modelBuilder.Entity<TEntity>().ConfigureCSumByConvention();

            ConfigureGlobalFilters<TEntity>(modelBuilder, mutableEntityType);
        }
    }
}
