using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Data.Filter;
using Volo.Abp.Domain.Entities;

namespace Volo.Abp.Domain.Repositories
{
    /// <summary>
    /// 支持数据过滤表达式查询
    /// </summary>
    public interface ISupportsPredicateQuery<TEntity>
         where TEntity : class, IEntity
    {
        /// <summary>
        /// 获取<typeparamref name="TEntity"/>查询(不跟踪改变的)数据源，并可附加过滤条件模型
        /// </summary>
        /// <param name="model">数据过滤模型</param>
        /// <param name="includeDetails">是否包含明细，默认true</param>
        /// <returns>符合条件的数据集</returns>
        IQueryable<TEntity> CreateFilteredQuery(IFilterModel model = null, Func<IQueryable<TEntity>, IFilterModel, IQueryable<TEntity>> func = null, bool includeDetails = true);

        /// <summary>
        /// 获取<typeparamref name="TEntity"/>跟踪数据更改（Tracking）的查询数据源，并可附加过滤条件
        /// </summary>
        /// <param name="predicate">数据过滤表达式</param>
        /// <returns>符合条件的数据集</returns>
        [Obsolete("Abp仓储基类原本就支持")]
        IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> predicate = null);

        /// <summary>
        /// 获取<typeparamref name="TEntity"/>跟踪数据更改（Tracking）的查询数据源，并可Include导航属性
        /// </summary>
        /// <param name="includePropertySelectors"></param>
        /// <returns>符合条件的数据集</returns>
        [Obsolete("Abp仓储基类原本就支持")]
        IQueryable<TEntity> GetQueryable(params Expression<Func<TEntity, object>>[] includePropertySelectors);

        /// <summary>
        /// 查找符合条件的数据集
        /// </summary>
        /// <param name="predicate">数据过滤表达式</param>
        /// <param name="includeDetails">包含明细</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns>符合条件的数据集</returns>
        Task<List<TEntity>> GetListAsync([NotNull]Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// 查找第一个符合条件的数据
        /// </summary>
        /// <param name="predicate">数据过滤表达式</param>
        /// <param name="includeDetails">包含明细</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns>符合条件的实体，不存在时返回null</returns>
        Task<TEntity> GetFirstOrDefaultAsync([NotNull]Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取<typeparamref name="TEntity"/>查询过滤条件的数据源数量
        /// </summary>
        /// <param name="predicate">数据过滤表达式</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns>符合条件的数据数量</returns>
        [Obsolete("Abp仓储基类原本就支持")]
        Task<long> GetCountAsync([NotNull]Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        
    }

    /// <summary>
    /// 支持数据过滤表达式查询
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface ISupportsPredicateQuery<TEntity,TKey>
        where TEntity : class, IEntity<TKey>
    {
        /// <summary>
        /// 检查实体是否存在
        /// </summary>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <param name="id">实体的主键</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns></returns>
        Task<bool> CheckExistsAsync([NotNull]Expression<Func<TEntity, bool>> predicate, TKey id = default, CancellationToken cancellationToken = default);
    }
}
