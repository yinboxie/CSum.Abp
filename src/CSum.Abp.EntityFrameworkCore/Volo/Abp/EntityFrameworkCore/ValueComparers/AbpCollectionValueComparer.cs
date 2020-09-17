using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Volo.Abp.EntityFrameworkCore.ValueComparers
{
    public class AbpCollectionValueComparer<T> : ValueComparer<List<T>>
    {
        public AbpCollectionValueComparer()
            : base(
                  (d1, d2) => d1.SequenceEqual(d2),
                  d => d.Aggregate(0, (k, v) => HashCode.Combine(k, v.GetHashCode())),
                  d => d.ToList()
                  )
        {
        }
    }
}
