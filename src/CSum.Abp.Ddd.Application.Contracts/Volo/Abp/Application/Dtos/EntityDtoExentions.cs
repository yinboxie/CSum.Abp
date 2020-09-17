using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Data.Tree;
using Volo.Abp.Domain.Entities;

namespace Volo.Abp.Application.Dtos
{
    public static class EntityDtoExentions
    {
        /// <summary>
        /// 转换成树形
        /// </summary>
        /// <typeparam name="TEntityDto"></typeparam>
        /// <param name="dtos"></param>
        /// <param name="mapToTreeModel"></param>
        /// <returns></returns>
        public static ICollection<TreeModel> ToTree<TEntityDto>(
            [NotNull]this ICollection<TEntityDto> dtos,
            [NotNull]Func<TEntityDto, TreeModel> mapToTreeModel)
            where TEntityDto : IEntityDto
        {
            Check.NotNull(dtos, nameof(dtos));
            Check.NotNull(mapToTreeModel, nameof(mapToTreeModel));

            var list = dtos.Select(mapToTreeModel).ToList();
            return TreeHelper.ToTree(list);
        }
    }
}
