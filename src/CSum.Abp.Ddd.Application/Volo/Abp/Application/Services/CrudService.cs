using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Volo.Abp.Application.Services
{
    public abstract class CrudService<TEntity, TGetOutputDto, TKey, TCreateInput, TUpdateInput>
        : CrudService<TEntity, TGetOutputDto, TGetOutputDto, TKey, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TKey>
        where TGetOutputDto : IEntityDto<TKey>
    {
        protected CrudService(IRepository<TEntity, TKey> repository)
            : base(repository)
        {

        }
    }
    public abstract class CrudService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TCreateInput, TUpdateInput>
        : AbstractKeyCrudService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TKey>
        where TGetOutputDto : IEntityDto<TKey>
    {
        protected new IRepository<TEntity, TKey> Repository { get; }

        protected CrudService(IRepository<TEntity, TKey> repository)
        : base(repository)
        {
            Repository = repository;
        }

        protected override async Task DeleteByIdAsync(TKey id)
        {
            await Repository.DeleteAsync(id);
        }

        protected override async Task<TEntity> GetEntityByIdAsync(TKey id)
        {
            return await Repository.GetAsync(id);
        }

        /// <summary>
        /// 内部逻辑校验
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected override Task<bool> InternalValidateEntityForCreateOrUpdate(TEntity entity)
        {
            if (entity.GetType().HasImplementedRawGeneric(typeof(IMayHaveParent<>)))
            {
                var pi = entity.GetType().GetProperties().FirstOrDefault(p => p.Name == "ParentId");
                var value = pi.GetValue(entity, null);
                if (value != null && value.Equals(entity.Id))
                {
                    throw new UserFriendlyException("不能选择当前节点作为上级");
                }
            }
            return Task.FromResult(true);
        }

        protected override void MapToEntity(TUpdateInput updateInput, TEntity entity)
        {
            if (updateInput is IEntityDto<TKey> entityDto)
            {
                entityDto.Id = entity.Id;
            }

            base.MapToEntity(updateInput, entity);
        }

        protected override IQueryable<TEntity> ApplyDefaultSorting(IQueryable<TEntity> query)
        {
            if (typeof(TEntity).IsAssignableTo<ISortable>())
            {
                return query.OrderBy(e => ((ISortable)e).SortCode);
            }
            else if (typeof(TEntity).IsAssignableTo<ICreationAuditedObject>())
            {
                return query.OrderByDescending(e => ((ICreationAuditedObject)e).CreationTime);
            }
            else
            {
                return query.OrderByDescending(e => e.Id);
            }
        }
    }
}
