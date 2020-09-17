using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data.Filter;

namespace System.Data
{
    public static class DbConnectionExtensions
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="facade"></param>
        /// <param name="sql"></param>
        /// <param name="paged"></param>
        /// <param name="condition"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static DataTable SqlQueryPaged(this IDbConnection connect, string sql, PagedAndSortedResultRequestDto paged, FilteringCondition condition, out int total, IDbTransaction transaction = null)
        {
            var strSql = sql + condition.ConditionalText;
            var dparams = condition.GetDynamicParams();

            var totalSql = $"Select Count(1) From( {strSql}) As t";
            total = connect.ExecuteScalar(totalSql, dparams,transaction: transaction).ToInt();

            var sb = SqlPageSql(strSql, paged.Sorting, paged.SkipCount, paged.MaxResultCount);

            return connect.ExecuteReader(sb.ToString(), dparams, transaction: transaction).ToDataTable();
        }

        /// <summary>
        /// 组合分页字符串
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="sorting">排序</param>
        /// <param name="skipCount">偏移量</param>
        /// <param name="maxResultCount">最大结果集</param>
        /// <returns></returns>
        private static StringBuilder SqlPageSql(string strSql, string sorting, int skipCount, int maxResultCount)
        {
            var sb = new StringBuilder();
            if (sorting.IsNullOrWhiteSpace())
            {
                throw new Exception("分页查询，必须得有排序条件");
            }
            sb.Append($"select * from ( {strSql} ) T order by {sorting}  offset {skipCount} rows fetch next {maxResultCount} rows only");
            return sb;
        }
    }
}
