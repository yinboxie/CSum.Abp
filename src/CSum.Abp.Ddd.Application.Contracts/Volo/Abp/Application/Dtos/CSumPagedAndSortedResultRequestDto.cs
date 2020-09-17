using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Application.Dtos
{
    public class CSumPagedAndSortedResultRequestDto : PagedAndSortedResultRequestDto, IHasPagedRequestDto
    {
        public bool IsPaged { get; set; } = true;
    }
}
