using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Data.Filter;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Volo.Abp.Domain.Repositories.EntityFrameworkCore
{
    /// <summary>
    /// 框架扩展<see cref="EfCoreRepository<TDbContext, TEntity>"/>实现
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class CSumEfCoreRepository<TDbContext, TEntity> : EfCoreRepository<TDbContext, TEntity>,
        ISupportsSpecificQuery<TEntity>,
        ISupportsPredicateQuery<TEntity>,
        ISupportsBatchOperate<TEntity>
        where TDbContext : IEfCoreDbContext
        where TEntity : class, IEntity
    {
        protected virtual IFilterService FilterService => _filterServiceLazy.Value;
        private readonly Lazy<IFilterService> _filterServiceLazy;

        public CSumEfCoreRepository(IDbContextProvider<TDbContext> dbContextProvider)
            :base(dbContextProvider)
        {
            _filterServiceLazy = new Lazy<IFilterService>(
                () => ServiceProvider.GetRequiredService<IFilterService>()
            ) ;
        }

        #region  ISupportsSpecificQuery<TEntity>  实现
        /// <summary>
        /// 获取指定行版本号的数据列表
        /// </summary>
        /// <param name="rv">行版本号</param>
        /// <param name="includeDetails">是否包含明细</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns></returns>
        public virtual async Task<List<TEntity>> GetListByRowVersionAsync(long rv, bool includeDetails, CancellationToken cancellationToken)
        {
            if (!typeof(IHasRowVersion).IsAssignableFrom(typeof(TEntity)))
            {
                throw new Exception("不支持通过行版本号查询实体列表");
            }
            return includeDetails
                ? await WithDetails().Where(p => CSumDbFunctions.ConvertToInt64(((IHasRowVersion)p).RowVersion) > rv).ToListAsync(GetCancellationToken(cancellationToken))
                : await DbSet.Where(p => CSumDbFunctions.ConvertToInt64(((IHasRowVersion)p).RowVersion) > rv).ToListAsync(GetCancellationToken(cancellationToken));
        }
        #endregion

        #region ISupportsPredicateQuery<TEntity> 实现
        /// <summary>
        /// 获取<typeparamref name="TEntity"/>查询(不跟踪改变的)数据源，并可附加过滤条件模型
        /// </summary>
        /// <param name="model">数据过滤模型</param>
        /// <param name="includeDetails">是否包含明细，默认true</param>
        /// <returns>符合条件的数据集</returns>
        public virtual IQueryable<TEntity> CreateFilteredQuery(IFilterModel model = null, Func<IQueryable<TEntity>, IFilterModel,IQueryable<TEntity>> func = null, bool includeDetails = true)
        {
            var query = includeDetails?  WithDetails():  DbSet;
            if (func != null)
            {
                query = func(query, model);
            }
            if (model != null)
            {
                var condition = FilterService.Translate(model);
                if(!condition.ConditionalText.IsNullOrWhiteSpace())
                    query = query.WhereDynamic(condition.ConditionalText);
            }
            return query.AsNoTracking();
        }

        /// <summary>
        /// 获取<typeparamref name="TEntity"/>跟踪数据更改（Tracking）的查询数据源，并可附加过滤条件及是否启用数据权限过滤
        /// </summary>
        /// <param name="predicate">数据过滤表达式</param>
        /// <returns>符合条件的数据集</returns>
        public virtual IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate = null)
        {
            return DbSet.AsQueryable().WhereIf(predicate != null, predicate);
        }

        /// <summary>
        /// 获取<typeparamref name="TEntity"/>跟踪数据更改（Tracking）的查询数据源，并可Include导航属性
        /// </summary>
        /// <param name="includePropertySelectors">要Include操作的属性表达式</param>
        /// <returns>符合条件的数据集</returns>
        public virtual IQueryable<TEntity> GetQueryable(params Expression<Func<TEntity, object>>[] includePropertySelectors)
        {
            IQueryable<TEntity> query = DbSet.AsQueryable();
            if (includePropertySelectors == null || includePropertySelectors.Length == 0)
            {
                return query;
            }

            foreach (Expression<Func<TEntity, object>> selector in includePropertySelectors)
            {
                query = query.Include(selector);
            }
            return query;
        }

        /// <summary>
        /// 查找符合条件的数据集
        /// </summary>
        /// <param name="predicate">数据过滤表达式</param>
        /// <param name="includeDetails">包含明细</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns>符合条件的数据集</returns>
        public virtual async Task<List<TEntity>> GetListAsync([NotNull]Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            Check.NotNull(predicate, nameof(predicate));
            return await (includeDetails ? WithDetails() : DbSet).WhereIf(predicate != null, predicate).ToListAsync(GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// 查找第一个符合条件的数据
        /// </summary>
        /// <param name="predicate">数据过滤表达式</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns>符合条件的实体，不存在时返回null</returns>
        public virtual async Task<TEntity> GetFirstOrDefaultAsync([NotNull]Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            Check.NotNull(predicate, nameof(predicate));
            return await (includeDetails? WithDetails() : DbSet).FirstOrDefaultAsync(predicate,GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// 获取<typeparamref name="TEntity"/>查询过滤条件的数据源数量
        /// </summary>
        /// <param name="predicate">数据过滤表达式</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns>符合条件的数据数量</returns>
        public virtual async Task<long> GetCountAsync([NotNull]Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            Check.NotNull(predicate, nameof(predicate));
            return await DbSet.LongCountAsync(predicate,GetCancellationToken(cancellationToken));
        }
        #endregion

        #region ISupportsBatchOperate<TEntity> 实现

        /// <summary>
        /// 插入或更新实体
        /// </summary>
        /// <param name="entity">插入或更新的实体</param>
        /// <param name="autoSave">是否自动保存</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns></returns>
        public virtual async Task InsertOrUpdateAsync([NotNull]TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            Check.NotNull(entity, nameof(entity));
            var model = await DbContext.FindAsync<TEntity>(entity.GetKeys(), GetCancellationToken(cancellationToken));
            if (model != null)
            {
                //Find实体时已经获得了跟踪对象，先去除跟踪状态
                var entry = DbContext.ChangeTracker.Entries().First(p => p.Entity == model);
                entry.State = EntityState.Detached;
                
                //再提交更新
                model = entity;
                DbContext.Update(model);
            }
            else
            {
                await DbContext.AddAsync(entity, GetCancellationToken(cancellationToken));
            }

            if (autoSave)
            {
                await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
            }
        }
        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <param name="entities">插入的实体列表</param>
        /// <param name="autoSave">是否自动保存</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns></returns>
        public virtual async Task InsertAsync([NotNull]IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            Check.NotNull(entities, nameof(entities));

            await DbContext.AddRangeAsync(entities, GetCancellationToken(cancellationToken));
            if (autoSave)
            {
                await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
            }
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">更新的实体列表</param>
        /// <param name="autoSave">是否自动保存</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns></returns>
        public virtual async Task UpdateAsync([NotNull]IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            Check.NotNull(entities, nameof(entities));

            //TODO: 是否需要先执行 AttachRange
            DbContext.UpdateRange(entities);
            if (autoSave)
            {
                await DbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
            }
        }

        /// <summary>
        /// 批量更新所有符合特定条件的实体
        /// </summary>
        /// <param name="predicate">查询条件的谓语表达式</param>
        /// <param name="updateExpression">属性更新表达式</param>
        /// <returns>操作影响的行数</returns>
        public virtual async Task<int> UpdateBatchAsync([NotNull]Expression<Func<TEntity, bool>> predicate, 
            [NotNull]Expression<Func<TEntity, TEntity>> updateExpression,
            CancellationToken cancellationToken = default)
        {
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(updateExpression, nameof(updateExpression));

            return await DbSet.Where(predicate).UpdateAsync(updateExpression, GetCancellationToken(cancellationToken));
        }

        /// <summary>
        /// 删除所有符合特定条件的实体
        /// </summary>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <returns>操作影响的行数</returns>
        public virtual async Task<int> DeleteBatchAsync([NotNull]Expression<Func<TEntity, bool>> predicate,
             CancellationToken cancellationToken = default)
        {
            Check.NotNull(predicate, nameof(predicate));
            return await DbSet.Where(predicate).DeleteAsync(GetCancellationToken(cancellationToken));
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 获取实体映射的表名
        /// </summary>
        /// <returns></returns>
        protected virtual string GetTableName()
        {
            var mapping = DbContext.Model.FindEntityType(typeof(TEntity));
            var schema = mapping.GetSchema();
            var tableName = mapping.GetTableName();
            return $"{schema}.{tableName}";
        }
        #endregion
    }

    /// <summary>
    /// 框架扩展<see cref="IEfCoreRepository<TEntity,TKey>"/>实现
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class CSumEfCoreRepository<TDbContext, TEntity, TKey> : CSumEfCoreRepository<TDbContext, TEntity>,
        IEfCoreRepository<TEntity, TKey>,
        ISupportsExplicitLoading<TEntity, TKey>,
        ISupportsSpecificQuery<TEntity, TKey>,
        ISupportsPredicateQuery<TEntity,TKey>
        where TDbContext : IEfCoreDbContext
        where TEntity : class, IEntity<TKey>
    {
        public CSumEfCoreRepository(IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        #region ISupportsPredicateQuery<TEntity,TKey> 实现
        /// <summary>
        /// 检查实体是否存在
        /// </summary>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <param name="id">实体的主键</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns></returns>
        public async Task<bool> CheckExistsAsync([NotNull]Expression<Func<TEntity, bool>> predicate, TKey id = default(TKey), CancellationToken cancellationToken = default)
        {
            Check.NotNull(predicate, nameof(predicate));

            TKey defaultId = default(TKey);
             var entity = await DbSet.Where(predicate).Select(m => new { m.Id }).FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
            bool exists = !typeof(TKey).IsValueType && ReferenceEquals(id, null) || id.Equals(defaultId)
                ? entity != null
                : entity != null && !entity.Id.Equals(id);
            return exists;
        }
        #endregion

        #region IReadOnlyBasicRepository<TEntity, TKey> 实现
        /// <summary>
        /// 通过主键值查询实体，不存在时会抛出<see cref="EntityNotFoundException"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            var entity = await FindAsync(id, includeDetails, GetCancellationToken(cancellationToken));

            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id);
            }

            return entity;
        }

        /// <summary>
        /// 通过主键值查询实体,不存在时返回null
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeDetails"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return includeDetails
                ? await WithDetails().FirstOrDefaultAsync(e => e.Id.Equals(id), GetCancellationToken(cancellationToken))
                : await DbSet.FindAsync(new object[] { id }, GetCancellationToken(cancellationToken));
        }
        #endregion

        #region  IBasicRepository<TEntity, TKey> 实现
        /// <summary>
        /// 通过主键值删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <param name="autoSave"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            var entity = await FindAsync(id, cancellationToken: cancellationToken);
            if (entity == null)
            {
                return;
            }

            await DeleteAsync(entity, autoSave, cancellationToken);
        }
        #endregion
    }
}
