using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore;

namespace Volo.Abp.Domain.Repositories.EntityFrameworkCore
{
    public class CSumEfCoreSqlServerRepository<TDbContext, TEntity> : CSumEfCoreRepository<TDbContext, TEntity>
         where TDbContext : IEfCoreDbContext
         where TEntity : class, IEntity
    {
        public CSumEfCoreSqlServerRepository(IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }

    /// <summary>
    /// 框架扩展<see cref="IEfCoreRepository<TEntity,TKey>"/>实现
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class CSumEfCoreSqlServerRepository<TDbContext, TEntity, TKey> : CSumEfCoreRepository<TDbContext, TEntity, TKey>,
        ISupportsSqlQuery<TEntity, TKey>
        where TDbContext : IEfCoreDbContext
        where TEntity : class, IEntity<TKey>
    {
        private CSumEfCoreSqlServerRepository<TDbContext, TEntity> _sqlRepository;
        public CSumEfCoreSqlServerRepository(IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
            _sqlRepository = new CSumEfCoreSqlServerRepository<TDbContext, TEntity>(dbContextProvider);
        }

        #region ISupportsSqlQuery<TEntity, TKey> 实现
        /// <summary>
        /// 递归获取实体TKey列表
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<TKey>> GetLoopIdsAsync(TKey id)
        {
            var ls = await GetLoopListAsync(id);
            return ls.Select(p => p.Id).ToList();
        }

        /// <summary>
        /// 递归获取实体列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<List<TEntity>> GetLoopListAsync(TKey id)
        {
            var tableName = GetTableName();
            var strSql = new StringBuilder(string.Format(
               @";WITH q AS 
                (
                SELECT * FROM {0} a WHERE 1=1 @where
                UNION ALL
                SELECT a.* FROM {0} a 
                INNER JOIN q b ON b.Id=a.ParentId
                )
                SELECT * 
                FROM q 
                ", tableName));
            var strWhere = new StringBuilder(" and (a.Id = @id or a.ParentId = @id )");
            strSql = strSql.Replace("@where", strWhere.ToString());
            var parameter = new List<DbParameter>();
            parameter.Add(new SqlParameter { ParameterName = "@id", Value = id });
            var dt = DbContext.Database.SqlQuery(strSql.ToString(), parameter.ToArray());
            var ls = dt.ToList<TEntity>();
            return Task.FromResult(ls);
        }
        #endregion
    }
}
