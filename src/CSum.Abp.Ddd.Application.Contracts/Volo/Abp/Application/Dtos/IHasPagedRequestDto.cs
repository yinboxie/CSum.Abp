using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.Application.Dtos
{
    public interface IHasPagedRequestDto
    {
        bool IsPaged { get; set; }
    }
}
