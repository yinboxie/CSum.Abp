using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data.Filter;

namespace Volo.Abp.Application.Services
{
    public interface ICrudService<TGetOutputDto, in TKey, in TCreateInput, in TUpdateInput>
        : ICrudService<TGetOutputDto, TGetOutputDto, TKey, TCreateInput, TUpdateInput>
    {

    }

    public interface ICrudService<TGetOutputDto, TGetListOutputDto, in TKey, in TCreateInput, in TUpdateInput>
       : IApplicationService
    {
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TGetOutputDto> GetAsync(TKey id);

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="model">数据过滤模型</param>
        /// <returns></returns>
        Task<List<TGetListOutputDto>> GetListAsync(IFilterModel model);

        /// <summary>
        /// 获取实体分页列表
        /// </summary>
        /// <param name="pagedRequestDto">分页请求模型</param>
        /// <param name="model">数据过滤模型</param>
        /// <returns></returns>
        Task<PagedResultDto<TGetListOutputDto>> GetPageListAsync(PagedAndSortedResultRequestDto pagedRequestDto, IFilterModel model);

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="input">创建实体模型</param>
        /// <returns></returns>
        Task<TGetOutputDto> CreateAsync(TCreateInput input);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <param name="input">更新实体模型</param>
        /// <returns></returns>
        Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput input);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <returns></returns>
        Task DeleteAsync(TKey id);

        /// <summary>
        /// 切换实体有效状态
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <param name="isActive">是否有效状态</param>
        /// <returns></returns>
        Task ToggleActiveAsync(TKey id, bool? isActive = null);
    }
}
