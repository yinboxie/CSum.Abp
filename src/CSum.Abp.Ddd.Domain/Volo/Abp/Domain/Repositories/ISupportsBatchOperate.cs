using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Volo.Abp.Domain.Repositories
{
    public interface ISupportsBatchOperate<TEntity>
         where TEntity : class, IEntity
    {

        /// <summary>
        /// 插入或更新实体
        /// </summary>
        /// <param name="entity">插入或更新的实体</param>
        /// <param name="autoSave">是否自动保存</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns></returns>
        Task InsertOrUpdateAsync([NotNull]TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <param name="entities">插入的实体列表</param>
        /// <param name="autoSave">是否自动保存</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns></returns>
        Task InsertAsync([NotNull]IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">更新的实体列表</param>
        /// <param name="autoSave">是否自动保存</param>
        /// <param name="cancellationToken">取消线程标识</param>
        /// <returns></returns>
        Task UpdateAsync([NotNull]IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// <para>批量更新所有符合特定条件的实体 使用EfCore.Plus 的扩展函数，不会触发实体的更新事件</para> 
        /// <para>实体更新之后，可以立即查询得到,不需要执行SaveChanges</para> 
        /// </summary>
        /// <param name="predicate">查询条件的谓语表达式</param>
        /// <param name="updateExpression">属性更新表达式</param>
        /// <returns>操作影响的行数</returns>
        Task<int> UpdateBatchAsync([NotNull]Expression<Func<TEntity, bool>> predicate,
            [NotNull]Expression<Func<TEntity, TEntity>> updateExpression,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// <para>删除所有符合特定条件的实体, 使用EfCore.Plus 的扩展函数，不会触发实体的删除事件</para> 
        /// <para>删除之后，不需要执行SaveChanges,使用Count或GetList查询不到数据</para>
        /// </summary>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <returns>操作影响的行数</returns>
        Task<int> DeleteBatchAsync([NotNull]Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);
    }
}
