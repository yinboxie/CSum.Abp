using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Data.Tree;

namespace Volo.Abp.Domain.Entities
{
    public static class EntityExentions
    {
        /// <summary>
        /// 转换成树形
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <param name="mapToTreeModel"></param>
        /// <returns></returns>
        public static ICollection<TreeModel> ToTree<TEntity>(
            [NotNull]this ICollection<TEntity> entities,
            [NotNull]Func<TEntity, TreeModel> mapToTreeModel)
            where TEntity : IEntity
        {
            Check.NotNull(entities, nameof(entities));
            Check.NotNull(mapToTreeModel, nameof(mapToTreeModel));

            var list = entities.Select(mapToTreeModel).ToList();
            return TreeHelper.ToTree(list);
        }

        /// <summary>
        /// 正向递归遍历(以Id作为parentId向下遍历)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static ICollection<TEntity> Recursive<TEntity,TKey>(
            [NotNull] this ICollection<TEntity> entities,
            [NotNull] TKey parentId)
             where TEntity : IEntity<TKey>, IMayHaveParent<TKey>
        {
            Check.NotNull(entities, nameof(entities));

            var query = from c in entities
                        where c.ParentId.Equals(parentId)
                        select c;
            var list = query.ToList();
            return query.ToList().Concat(query.ToList().SelectMany(t => Recursive(entities, t.Id))).ToList();
        }

        /// <summary>
        /// 反向递归遍历(以ParentId作为Id向上遍历)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entities"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static ICollection<TEntity> RecursiveReverse<TEntity, TKey>(
            [NotNull] this ICollection<TEntity> entities,
            [NotNull] TKey parentId)
             where TEntity : IEntity<TKey>, IMayHaveParent<TKey>
        {
            Check.NotNull(entities, nameof(entities));

            var query = from c in entities
                        where c.Id.Equals(parentId)
                        select c;
            var list = query.ToList();
            return query.ToList().Concat(query.ToList().SelectMany(t => RecursiveReverse(entities, t.ParentId))).ToList();
        }
    }
}
