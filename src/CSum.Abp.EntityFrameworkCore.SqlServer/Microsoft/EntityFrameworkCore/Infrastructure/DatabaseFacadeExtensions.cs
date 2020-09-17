using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data.Filter;

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    public static class DatabaseFacadeExtensions
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
        public static DataTable SqlQueryPaged(this DatabaseFacade facade, string sql, PagedAndSortedResultRequestDto paged, FilteringCondition condition, out int total)
        {
            var strSql = sql + condition.ConditionalText;

            var parameter = new List<DbParameter>();
            foreach (var item in condition.FilteringParams)
            {
                parameter.Add(new SqlParameter { ParameterName = item.Name, Value = item.Value });
            }

            var parameter2 = new DbParameter[parameter.Count];
            parameter.CopyTo(parameter2);

            var totalSql = $"Select Count(1) From( {strSql}) As t";
            total = facade.ExecuteScalar(totalSql, parameter.ToArray()).ToInt();

            var sb = SqlPageSql(strSql, paged.Sorting, paged.SkipCount, paged.MaxResultCount);

            return facade.SqlQuery(sb.ToString(), parameter2);
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
            sb.Append($"select * from ( {strSql} ) T order by {sorting} offset {skipCount} rows fetch next {maxResultCount} rows only");
            return sb;
        }
    }
}
