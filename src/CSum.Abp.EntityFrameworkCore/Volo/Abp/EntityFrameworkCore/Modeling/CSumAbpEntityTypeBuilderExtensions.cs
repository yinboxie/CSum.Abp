using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Volo.Abp.EntityFrameworkCore.Modeling
{
    public static class CSumAbpEntityTypeBuilderExtensions
    {
        public static void ConfigureCSumByConvention(this EntityTypeBuilder b)
        {
            b.ConfigureByConvention();
            b.TryConfigureSimpleSeplling();
            b.TryConfigurePassivable();
            b.TryConfigureSortable();
            b.TryConfigureRowVersion();
            b.TryCSumConfigureCreationTime();
        }

        /// <summary>
        /// 配置<see cref="IHasSimpleSpelling"/>
        /// </summary>
        /// <param name="b"></param>
        public static void TryConfigureSimpleSeplling(this EntityTypeBuilder b)
        {
            if (b.Metadata.ClrType.IsAssignableTo<IHasSimpleSpelling>())
            {
                b.Property(nameof(IHasSimpleSpelling.SimpleSpelling))
                    .HasMaxLength(128)
                    .HasColumnName(nameof(IHasSimpleSpelling.SimpleSpelling));
            }
        }

        /// <summary>
        ///  配置<see cref="IPassivable"/>,IsActive默认值为true
        /// </summary>
        /// <param name="b"></param>
        public static void TryConfigurePassivable(this EntityTypeBuilder b)
        {
            if (b.Metadata.ClrType.IsAssignableTo<IPassivable>())
            {
                b.Property(nameof(IPassivable.IsActive))
                    .HasDefaultValue(true)
                    .HasColumnName(nameof(IPassivable.IsActive));
            }
        }

        /// <summary>
        /// 配置<see cref="ISortable"/>,SortCode默认值为99
        /// </summary>
        /// <param name="b"></param>
        public static void TryConfigureSortable(this EntityTypeBuilder b)
        {
            if (b.Metadata.ClrType.IsAssignableTo<ISortable>())
            {
                b.Property(nameof(ISortable.SortCode))
                    .HasDefaultValue(99)
                    .HasColumnName(nameof(ISortable.SortCode));
            }
        }

        /// <summary>
        /// 配置<see cref="IHasRowVersion"/>
        /// </summary>
        /// <param name="b"></param>
        public static void TryConfigureRowVersion(this EntityTypeBuilder b)
        {
            if (b.Metadata.ClrType.IsAssignableTo<IHasRowVersion>())
            {
                b.Property(nameof(IHasRowVersion.RowVersion)).IsRowVersion();
            }
        }

        /// <summary>
        /// 配置<see cref="IHasCreationTime"/>
        /// </summary>
        /// <param name="b"></param>
        public static void TryCSumConfigureCreationTime(this EntityTypeBuilder b)
        {
            if (b.Metadata.ClrType.IsAssignableTo<IHasCreationTime>())
            {
                b.Property(nameof(IHasCreationTime.CreationTime))
                    .IsRequired()
                    .HasColumnName(nameof(IHasCreationTime.CreationTime))
                    .HasDefaultValueSql("getdate()");
            }
        }
    }
}
