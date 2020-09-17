using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Data.Filter;
using Volo.Abp.Domain.Entities;
using Volo.Abp.DynamicProxy;

namespace Volo.Abp.Domain.Repositories
{
    /// <summary>
    /// 仓储扩展
    /// </summary>
    public static class RepositoryExtensions
    {
        #region ISupportsSpecificQuery<TEntity>  实现扩展
        /// <summary>
        /// 获取指定行版本号的数据列表
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="rv">指定行版本号</param>
        /// <param name="includeDetails">是否包含明细</param>
        /// <param name="cancellationToken"></param>
        /// <returns>大于指定行版本号的数据集合</returns>
        public static async Task<List<TEntity>> GetListByRowVersionAsync<TEntity>(
           this IReadOnlyRepository<TEntity> repository,
           long rv,
           bool includeDetails = false,
           CancellationToken cancellationToken = default)
           where TEntity : class, IEntity
        {
            if (!typeof(IHasRowVersion).IsAssignableFrom(typeof(TEntity)))
            {
                throw new Exception($"实体类型不支持该扩展方法({MethodBase.GetCurrentMethod().DeclaringType.Name})");
            }

            var repo = ProxyHelper.UnProxy(repository) as ISupportsSpecificQuery<TEntity>;
            return repo != null ? await repo.GetListByRowVersionAsync(rv, includeDetails, cancellationToken) : null;
        }
        #endregion

        #region ISupportsSqlQuery<TEntity,TKey>  实现扩展
        /// <summary>
        /// 递归获取实体TKey列表
        /// </summary>
        /// <typeparam name="TEntity">必须继承<see cref="IMayHaveParent<TKey>"/></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="repository"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<List<TKey>> GetLoopIdsAsync<TEntity, TKey>(
           this IReadOnlyBasicRepository<TEntity, TKey> repository,
           TKey id)
           where TEntity : class, IEntity<TKey>
        {
            if (!typeof(TEntity).HasImplementedRawGeneric(typeof(IMayHaveParent<>)))
            {
                throw new Exception($"实体类型不支持该扩展方法({MethodBase.GetCurrentMethod().DeclaringType.Name})");
            }
            var repo = ProxyHelper.UnProxy(repository) as ISupportsSqlQuery<TEntity, TKey>;
            return repo != null ? await repo.GetLoopIdsAsync(id) : null;
        }

        /// <summary>
        /// 递归获取实体列表
        /// </summary>
        /// <typeparam name="TEntity">必须继承<see cref="IMayHaveParent<TKey>"/></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="repository"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<List<TEntity>> GetLoopListAsync<TEntity, TKey>(
           this IReadOnlyBasicRepository<TEntity, TKey> repository,
           TKey id)
           where TEntity : class, IEntity<TKey>
        {
            if (!typeof(TEntity).HasImplementedRawGeneric(typeof(IMayHaveParent<>)))
            {
                throw new Exception($"实体类型不支持该扩展方法({MethodBase.GetCurrentMethod().DeclaringType.Name})");
            }
            var repo = ProxyHelper.UnProxy(repository) as ISupportsSqlQuery<TEntity, TKey>;
            return repo != null ? await repo.GetLoopListAsync(id) : null;
        }
        #endregion

        #region ISupportsPredicateQuery<TEntity> 实现扩展
        /// <summary>
        /// 获取<typeparamref name="TEntity"/>查询(不跟踪改变的)数据源，并可附加过滤条件模型
        /// </summary>
        /// <param name="model">数据过滤模型</param>
        /// <param name="includeDetails">是否包含明细，默认true</param>
        /// <returns>符合条件的数据集</returns>
        public static IQueryable<TEntity> CreateFilteredQuery<TEntity>(this IReadOnlyBasicRepository<TEntity> repository, 
            IFilterModel model = null,
            Func<IQueryable<TEntity>, IFilterModel, IQueryable<TEntity>> func = null,
            bool includeDetails = true)
            where TEntity : class, IEntity
        {
            var repo = ProxyHelper.UnProxy(repository) as ISupportsPredicateQuery<TEntity>;
            return repo != null ? repo.CreateFilteredQuery(model, func, includeDetails) : null;
        }

        /// <summary>
        /// 获取<typeparamref name="TEntity"/>跟踪数据更改（Tracking）的查询数据源，并可附加过滤条件
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="predicate">数据过滤表达式</param>
        /// <returns>符合条件的数据集</returns>
        [Obsolete("Abp仓储基类 IReadOnlyBasicRepository 继承了 IQueryable")]
        public static IQueryable<TEntity> GetQueryable<TEntity>(
           this IReadOnlyBasicRepository<TEntity> repository,
           Expression<Func<TEntity, bool>> predicate = null)
           where TEntity : class, IEntity
        {
            var repo = ProxyHelper.UnProxy(repository) as ISupportsPredicateQuery<TEntity>;
            return repo != null ? repo.GetQueryable(predicate) : null;
        }

        /// <summary>
        /// 获取<typeparamref name="TEntity"/>跟踪数据更改（Tracking）的查询数据源，并可Include导航属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="includePropertySelectors"></param>
        /// <returns>符合条件的数据集</returns>
        [Obsolete("Abp仓储基类原本就支持，使用 WithDetails替代")]
        public static IQueryable<TEntity> GetQueryable<TEntity>(
           this IReadOnlyBasicRepository<TEntity> repository,
           params Expression<Func<TEntity, object>>[] includePropertySelectors)
           where TEntity : class, IEntity
        {
            var repo = ProxyHelper.UnProxy(repository) as ISupportsPredicateQuery<TEntity>;
            return repo != null ? repo.GetQueryable(includePropertySelectors) : null;
        }

        /// <summary>
        /// 查找符合条件的数据集
        /// </summary>
        /// <param name="predicate">数据过滤表达式</param>
        /// <param name="includeDetails">包含明细</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns>符合条件的数据集</returns>
        public static async Task<List<TEntity>> GetListAsync<TEntity>(
            this IReadOnlyBasicRepository<TEntity> repository,
            [NotNull]Expression<Func<TEntity, bool>> predicate, 
            bool includeDetails = true, 
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            Check.NotNull(predicate, nameof(predicate));
            var repo = ProxyHelper.UnProxy(repository) as ISupportsPredicateQuery<TEntity>;
            return repo != null ? await repo.GetListAsync(predicate, includeDetails, cancellationToken): null;
        }

        /// <summary>
        /// 查找第一个符合条件的数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="predicate">数据过滤表达式</param>
        /// <param name="includeDetails">包含明细</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <remarks>IQueryable扩展函数 FirstOrDefaultAsync 不具备查明细功能，所以此扩展方法有用</remarks>
        /// <returns>符合条件的实体，不存在时返回null</returns>
        public static async Task<TEntity> GetFirstOrDefaultAsync<TEntity>(
           this IReadOnlyRepository<TEntity> repository,
           [NotNull]Expression<Func<TEntity, bool>> predicate,
           bool includeDetails = true,
           CancellationToken cancellationToken = default)
           where TEntity : class, IEntity
        {
            Check.NotNull(predicate, nameof(predicate));
            var repo = ProxyHelper.UnProxy(repository) as ISupportsPredicateQuery<TEntity>;
            return repo != null ? await repo.GetFirstOrDefaultAsync(predicate, includeDetails, cancellationToken) : null;
        }

        /// <summary>
        /// 获取<typeparamref name="TEntity"/>查询过滤条件的数据源数量
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <param name="predicate">数据过滤表达式</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns>符合条件的数据数量</returns>
        [Obsolete("使用EfCore IQueryable扩展函数 CountAsync替代")]
        public static async Task<long> GetCountAsync<TEntity>(
            this IReadOnlyBasicRepository<TEntity> repository,
            [NotNull]Expression<Func<TEntity, bool>> predicate, 
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            Check.NotNull(predicate, nameof(predicate));
            var repo = ProxyHelper.UnProxy(repository) as ISupportsPredicateQuery<TEntity>;
            return repo != null ? await repo.GetCountAsync(predicate, cancellationToken) : 0;
        }
        #endregion

        #region ISupportsPredicateQuery<TEntity,TKey> 实现扩展
        /// <summary>
        /// 检查实体是否存在
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="repository"></param>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <param name="id">实体的主键</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns></returns>
        public static async Task<bool> CheckExistsAsync<TEntity,TKey>(
            this IReadOnlyBasicRepository<TEntity,TKey> repository,
            [NotNull]Expression<Func<TEntity, bool>> predicate,
            TKey id = default,
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity<TKey>
        {
            Check.NotNull(predicate, nameof(predicate));
            var repo = ProxyHelper.UnProxy(repository) as ISupportsPredicateQuery<TEntity, TKey>;
            return repo != null ? await repo.CheckExistsAsync(predicate, id, cancellationToken): false;
        }
        #endregion

        #region ISupportsBatchOperate<TEntity> 实现扩展
        /// <summary>
        /// 插入或更新实体
        /// </summary>
        /// <param name="entity">插入或更新的实体</param>
        /// <param name="autoSave">是否自动保存</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns></returns>
        public static async Task InsertOrUpdateAsync<TEntity>(this IBasicRepository<TEntity> repository, 
            [NotNull]TEntity entity, 
            bool autoSave = false, 
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            Check.NotNull(entity, nameof(entity));
            var repo = ProxyHelper.UnProxy(repository) as ISupportsBatchOperate<TEntity>;
            if (repo != null)
                await repo.InsertOrUpdateAsync(entity, autoSave, cancellationToken);
        }

        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <param name="entities">插入的实体列表</param>
        /// <param name="autoSave">是否自动保存</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns></returns>
        public static async Task InsertAsync<TEntity>(
            this IBasicRepository<TEntity> repository,
            [NotNull]IEnumerable<TEntity> entities, 
            bool autoSave = false, 
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            Check.NotNull(entities, nameof(entities));
            var repo = ProxyHelper.UnProxy(repository) as ISupportsBatchOperate<TEntity>;
            if (repo != null)
                await repo.InsertAsync(entities, autoSave, cancellationToken);
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">更新的实体列表</param>
        /// <param name="autoSave">是否自动保存</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns></returns>
        public static async Task UpdateAsync<TEntity>(
            this IBasicRepository<TEntity> repository,
            [NotNull]IEnumerable<TEntity> entities, 
            bool autoSave = false, 
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            Check.NotNull(entities, nameof(entities));
            var repo = ProxyHelper.UnProxy(repository) as ISupportsBatchOperate<TEntity>;
            if (repo != null)
                await repo.UpdateAsync(entities, autoSave, cancellationToken);
        }

        /// <summary>
        /// 批量更新所有符合特定条件的实体，使用EfCore.Plus 的扩展函数，不会触发实体的更新事件
        /// </summary>
        /// <param name="predicate">查询条件的谓语表达式</param>
        /// <param name="updateExpression">属性更新表达式</param>
        /// <returns>操作影响的行数</returns>
        public static async Task<int> UpdateBatchAsync<TEntity>(
            this IBasicRepository<TEntity> repository,
            [NotNull]Expression<Func<TEntity, bool>> predicate,
            [NotNull]Expression<Func<TEntity, TEntity>> updateExpression,
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(updateExpression, nameof(updateExpression));
            var repo = ProxyHelper.UnProxy(repository) as ISupportsBatchOperate<TEntity>;
            return repo != null ? await repo.UpdateBatchAsync(predicate, updateExpression, cancellationToken) : 0;
        }
        /// <summary>
        /// 删除所有符合特定条件的实体，使用EfCore.Plus 的扩展函数，不会触发实体的删除事件
        /// </summary>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <returns>操作影响的行数</returns>
        public static async Task<int> DeleteBatchAsync<TEntity>(
            this IBasicRepository<TEntity> repository,
            [NotNull]Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            Check.NotNull(predicate, nameof(predicate));
            var repo = ProxyHelper.UnProxy(repository) as ISupportsBatchOperate<TEntity>;
            return repo != null ? await repo.DeleteBatchAsync(predicate, cancellationToken) : 0;
        }
        #endregion

    }
}
