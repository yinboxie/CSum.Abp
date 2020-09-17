using JetBrains.Annotations;
using System;

namespace Volo.Abp.Application.Dtos
{
    public static class PageExtensions
    {
        public static CSumPagedResultDto ToCSumPagedResult<T>([NotNull] this PagedResultDto<T> result)
        {
            Check.NotNull(result, nameof(result));
            var dto = new CSumPagedResultDto();
            dto.rows = result.Items;
            dto.total = result.TotalCount;
            return dto;
        }

        public static PagedAndSortedResultRequestDto ToPagedRequest([NotNull] this CSumPagedRequestDto requestDto)
        {
            var paged = new CSumPagedAndSortedResultRequestDto();
            if (!requestDto.sidx.IsNullOrWhiteSpace())
            {
                paged.Sorting = $"{requestDto.sidx.Trim()} {requestDto.sord.Trim()}";
            }
            paged.MaxResultCount = requestDto.rows > 0 ? requestDto.rows : LimitedResultRequestDto.MaxMaxResultCount;
            if (requestDto.page > 0)
            {
                paged.SkipCount = requestDto.rows * (requestDto.page - 1);
            }
            else
            {
                paged.IsPaged = false;
            }
            return paged;
        }
    }
}
