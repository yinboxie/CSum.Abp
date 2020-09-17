using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Volo.Abp.Domain.Repositories
{
    /// <summary>
    /// 支持特定业务扩展的查询
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ISupportsSpecificQuery<TEntity>
         where TEntity : class, IEntity
    {
        /// <summary>
        /// 获取指定行版本号的数据列表
        /// </summary>
        /// <param name="rv">指定的行版本号</param>
        /// <param name="includeDetails">是否包含明细</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns></returns>
        Task<List<TEntity>> GetListByRowVersionAsync(long rv, bool includeDetails = false, CancellationToken cancellationToken = default);

        
    }

    /// <summary>
    /// 支持特定业务扩展的查询
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface ISupportsSpecificQuery<TEntity,TKey>
         where TEntity : class, IEntity<TKey>
    {

    }
}
