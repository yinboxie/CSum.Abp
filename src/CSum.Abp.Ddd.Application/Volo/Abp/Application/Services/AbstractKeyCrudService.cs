using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Authorization;
using Volo.Abp.Data.Filter;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.Application.Services
{
    public abstract class AbstractKeyCrudService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TCreateInput, TUpdateInput>
         : ApplicationService,
            ICrudService<TGetOutputDto, TGetListOutputDto, TKey, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity
    {
        protected IRepository<TEntity> Repository { get; }

        /// <summary>
        /// 查询资源策略名称
        /// </summary>
        protected virtual string GetPolicyName { get; set; }

        /// <summary>
        /// 查询列表/分页列表资源策略名称
        /// </summary>
        protected virtual string GetListPolicyName { get; set; }

        /// <summary>
        /// 创建资源策略名称
        /// </summary>
        protected virtual string CreatePolicyName { get; set; }

        /// <summary>
        /// 更新资源策略名称
        /// </summary>
        protected virtual string UpdatePolicyName { get; set; }

        /// <summary>
        /// 删除资源策略名称
        /// </summary>
        protected virtual string DeletePolicyName { get; set; }

        /// <summary>
        /// 默认的扩展过滤条件设置的委托
        /// </summary>
        protected virtual Func<IQueryable<TEntity>, IFilterModel, IQueryable<TEntity>> DefaultExtendFilterFunc { get; set; }

        /// <summary>
        /// 获取输出列表的委托
        /// </summary>
        protected virtual Func<IQueryable<TEntity>, List<TGetListOutputDto>> DefaultGetListOutputFunc { get; set; }

        protected AbstractKeyCrudService(IRepository<TEntity> repository)
        {
            Repository = repository;
        }

        #region Crud
        public virtual async Task<TGetOutputDto> GetAsync(TKey id)
        {
            await CheckGetPolicyAsync();

            var entity = await GetEntityByIdAsync(id);
            return MapToGetOutputDto(entity);
        }

        public virtual async Task<List<TGetListOutputDto>> GetListAsync(IFilterModel model = null)
        {
            return await GetListAsync(true, model);
        }

        protected virtual async Task<List<TGetListOutputDto>> GetListAsync(bool IgnoreDisabled, IFilterModel model = null)
        {
            await CheckGetListPolicyAsync();
            var query = CreateFilteredQuery(model);
            if (IgnoreDisabled)
            {
                if (typeof(TEntity).IsAssignableTo<IPassivable>())
                {
                    query =  query.Where(e => ((IPassivable)e).IsActive == true);
                }
            }
            query = ApplySorting(query);

            var func = CreateGetListOutputFunc(); // 输出的委托
            if (func != null)
            {
                return func(query);
            }
            else
            {
                var entities = await AsyncExecuter.ToListAsync(query);
                return entities.Select(MapToGetListOutputDto).ToList();
            }
        }

        public virtual async Task<PagedResultDto<TGetListOutputDto>> GetPageListAsync(PagedAndSortedResultRequestDto pagedRequestDto, IFilterModel model = null)
        {
            await CheckGetListPolicyAsync();
            var query = CreateFilteredQuery(model);

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = ApplySorting(query, pagedRequestDto);

            //如果有分页条件
            if (pagedRequestDto is IHasPagedRequestDto hasPaged)
            {
                if (hasPaged.IsPaged)
                {
                    //进行分页查询
                    query = ApplyPaging(query, pagedRequestDto);
                }
                else
                {
                    //不分页查询时,只查询有效数据
                    if (typeof(TEntity).IsAssignableTo<IPassivable>())
                    {
                        query = query.Where(e => ((IPassivable)e).IsActive == true);
                    }
                }
            }
            else
            {
                query = ApplyPaging(query, pagedRequestDto);
            } 

            List<TGetListOutputDto> listOutputDtos;
            var func = CreateGetListOutputFunc(); // 输出的委托
            if (func != null)
            {
                listOutputDtos = func(query);
            }
            else
            {
                var entities = await AsyncExecuter.ToListAsync(query);
                listOutputDtos = entities.Select(MapToGetListOutputDto).ToList();
            }
            return new PagedResultDto<TGetListOutputDto>(
                totalCount,
                listOutputDtos
            );
        }

        public virtual async Task<TGetOutputDto> CreateAsync(TCreateInput input)
        {
            await CheckCreatePolicyAsync();

            var entity = MapToEntity(input);

            await InternalValidateEntityForCreateOrUpdate(entity);
            await ValidateEntityForCreateOrUpdate(entity);

            SetIdForGuids(entity);
            TryToSetTenantId(entity);

            await Repository.InsertAsync(entity);

            return MapToGetOutputDto(entity);
        }

        public virtual async Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput input)
        {
            await CheckUpdatePolicyAsync();

            var entity = await GetEntityByIdAsync(id);
            MapToEntity(input, entity);

            await InternalValidateEntityForCreateOrUpdate(entity);
            await ValidateEntityForCreateOrUpdate(entity);

            await Repository.UpdateAsync(entity);

            return MapToGetOutputDto(entity);
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            await CheckDeletePolicyAsync();

            await ValidateEntityForDelete(id);

            await DeleteByIdAsync(id);
        }

        public virtual async Task ToggleActiveAsync(TKey id, bool? isActive = null)
        {
            if (!typeof(IPassivable).IsAssignableFrom(typeof(TEntity)))
            {
                throw new AbpException($"实体类型({typeof(TEntity).FullName})未继承IPassivable");
            }
            var entity = await GetEntityByIdAsync(id);
            ((IPassivable)entity).ToggleActive(isActive);
        }


        protected abstract Task DeleteByIdAsync(TKey id);

        protected abstract Task<TEntity> GetEntityByIdAsync(TKey id);

        /// <summary>
        /// 新增或编辑时校验
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual Task<bool> ValidateEntityForCreateOrUpdate(TEntity entity)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// 删除实体时校验，如果要阻止删除，直接抛出友好异常
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual Task<bool> ValidateEntityForDelete(TKey id)
        {
            return Task.FromResult(true);
        }

        protected abstract Task<bool> InternalValidateEntityForCreateOrUpdate(TEntity entity);
        #endregion

        #region IQueryable Apply 扩展
        /// <summary>
        /// This method should create <see cref="IQueryable{TEntity}"/> based on given input.
        /// It should filter query if needed, but should not do sorting or paging.
        /// Sorting should be done in <see cref="ApplySorting"/> and paging should be done in <see cref="ApplyPaging"/>
        /// methods.
        /// </summary>
        /// <param name="input">The input.</param>
        protected virtual IQueryable<TEntity> CreateFilteredQuery(IFilterModel model = null, bool includeDetails = true)
        {
            return Repository.CreateFilteredQuery(model, CreateExtendFilterFunc(), includeDetails) ;
        }

        /// <summary>
        /// 创建扩展过滤条件委托
        /// </summary>
        /// <returns></returns>
        protected virtual Func<IQueryable<TEntity>, IFilterModel, IQueryable<TEntity>> CreateExtendFilterFunc()
        {
            return DefaultExtendFilterFunc;
        }

        /// <summary>
        /// 创建输出列表委托
        /// </summary>
        /// <returns></returns>
        protected virtual Func<IQueryable<TEntity>, List<TGetListOutputDto>> CreateGetListOutputFunc()
        {
            return DefaultGetListOutputFunc;
        }

        /// <summary>
        /// Should apply sorting if needed.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="input">The input.</param>
        protected virtual IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, PagedAndSortedResultRequestDto input = null)
        {
            if (input == null)
            {
                return ApplyDefaultSorting(query);
            }

            //Try to sort query if available
            if (input is ISortedResultRequest sortInput)
            {
                if (!sortInput.Sorting.IsNullOrWhiteSpace())
                {
                    return query.OrderBy(sortInput.Sorting);
                }
            }

            //IQueryable.Task requires sorting, so we should sort if Take will be used.
            if (input is ILimitedResultRequest)
            {
                return ApplyDefaultSorting(query);
            }

            //No sorting
            return query;
        }

        /// <summary>
        /// Applies sorting if no sorting specified but a limited result requested.
        /// </summary>
        /// <param name="query">The query.</param>
        protected virtual IQueryable<TEntity> ApplyDefaultSorting(IQueryable<TEntity> query)
        {
            if (typeof(TEntity).IsAssignableTo<ICreationAuditedObject>())
            {
                return query.OrderByDescending(e => ((ICreationAuditedObject)e).CreationTime);
            }

            throw new AbpException("No sorting specified but this query requires sorting. Override the ApplyDefaultSorting method for your application service derived from AbstractKeyCrudAppService!");
        }

        /// <summary>
        /// Should apply paging if needed.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="input">The input.</param>
        protected virtual IQueryable<TEntity> ApplyPaging(IQueryable<TEntity> query, PagedAndSortedResultRequestDto input)
        {
            //Try to use paging if available
            if (input is IPagedResultRequest pagedInput)
            {
                return query.PageBy(pagedInput);
            }

            //Try to limit query result if available
            if (input is ILimitedResultRequest limitedInput)
            {
                return query.Take(limitedInput.MaxResultCount);
            }

            //No paging
            return query;
        }
        #endregion

        #region 实体映射
        /// <summary>
        /// Maps <see cref="TEntity"/> to <see cref="TGetListOutputDto"/>.
        /// It uses <see cref="IObjectMapper"/> by default.
        /// It can be overriden for custom mapping.
        /// </summary>
        protected virtual TGetListOutputDto MapToGetListOutputDto(TEntity entity)
        {
            return ObjectMapper.Map<TEntity, TGetListOutputDto>(entity);
        }
        /// <summary>
        /// Maps <see cref="TEntity"/> to <see cref="TGetOutputDto"/>.
        /// It uses <see cref="IObjectMapper"/> by default.
        /// It can be overriden for custom mapping.
        /// </summary>
        protected virtual TGetOutputDto MapToGetOutputDto(TEntity entity)
        {
            return ObjectMapper.Map<TEntity, TGetOutputDto>(entity);
        }
        /// <summary>
        /// Maps <see cref="TCreateInput"/> to <see cref="TEntity"/> to create a new entity.
        /// It uses <see cref="IObjectMapper"/> by default.
        /// It can be overriden for custom mapping.
        /// </summary>
        protected virtual TEntity MapToEntity(TCreateInput createInput)
        {
            var entity = ObjectMapper.Map<TCreateInput, TEntity>(createInput);
            return entity;
        }
        /// <summary>
        /// Maps <see cref="TUpdateInput"/> to <see cref="TEntity"/> to update the entity.
        /// It uses <see cref="IObjectMapper"/> by default.
        /// It can be overriden for custom mapping.
        /// </summary>
        protected virtual void MapToEntity(TUpdateInput updateInput, TEntity entity)
        {
            ObjectMapper.Map(updateInput, entity);
        }
        #endregion

        #region 权限判断
        protected virtual async Task CheckGetPolicyAsync()
        {
            await CheckPolicyAsync(GetPolicyName);
        }

        protected virtual async Task CheckGetListPolicyAsync()
        {
            await CheckPolicyAsync(GetListPolicyName);
        }

        protected virtual async Task CheckCreatePolicyAsync()
        {
            await CheckPolicyAsync(CreatePolicyName);
        }

        protected virtual async Task CheckUpdatePolicyAsync()
        {
            await CheckPolicyAsync(UpdatePolicyName);
        }

        protected virtual async Task CheckDeletePolicyAsync()
        {
            await CheckPolicyAsync(DeletePolicyName);
        }

        protected override async Task CheckPolicyAsync([CanBeNull] string policyName)
        {
            if (typeof(AlwaysAllowAuthorizationService) != ServiceProvider.GetService(typeof(IAuthorizationService)).GetType() 
                &&!CurrentUser.IsAuthenticated)
            {
                throw new AbpAuthorizationException("未登录");
            }

            if (string.IsNullOrEmpty(policyName))
            {
                return;
            }

            await AuthorizationService.CheckAsync(policyName);
        }
        #endregion

        #region 设置属性值
        /// <summary>
        /// Sets Id value for the entity if <see cref="TKey"/> is <see cref="Guid"/>.
        /// It's used while creating a new entity.
        /// </summary>
        protected virtual void SetIdForGuids(TEntity entity)
        {
            var entityWithGuidId = entity as IEntity<Guid>;

            if (entityWithGuidId == null || entityWithGuidId.Id != Guid.Empty)
            {
                return;
            }

            EntityHelper.TrySetId(
                entityWithGuidId,
                () => GuidGenerator.Create(),
                true
            );
        }
        protected virtual void TryToSetTenantId(TEntity entity)
        {
            if (entity is IMultiTenant && HasTenantIdProperty(entity))
            {
                var tenantId = CurrentTenant.Id;

                if (!tenantId.HasValue)
                {
                    return;
                }

                var propertyInfo = entity.GetType().GetProperty(nameof(IMultiTenant.TenantId));

                if (propertyInfo == null || propertyInfo.GetSetMethod(true) == null)
                {
                    return;
                }

                propertyInfo.SetValue(entity, tenantId);
            }
        }

        protected virtual bool HasTenantIdProperty(TEntity entity)
        {
            return entity.GetType().GetProperty(nameof(IMultiTenant.TenantId)) != null;
        }
        #endregion
    }
}