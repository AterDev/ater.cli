﻿using Share.Models.GenStepDtos;

namespace Application.Managers;
/// <summary>
/// task step
/// </summary>
public class GenStepManager(
    DataAccessContext<GenStep> dataContext, 
    ILogger<GenStepManager> logger,
    IUserContext userContext) : ManagerBase<GenStep>(dataContext, logger)
{
    private readonly IUserContext _userContext = userContext;

    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<Guid?> CreateNewEntityAsync(GenStepAddDto dto)
    {
        var entity = dto.MapTo<GenStepAddDto, GenStep>();
        // TODO:完善添加逻辑
        return await base.AddAsync(entity) ? entity.Id : null;
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(GenStep entity, GenStepUpdateDto dto)
    {
        entity.Merge(dto);
        // TODO:完善更新逻辑
        return await base.UpdateAsync(entity);
    }

    public async Task<PageList<GenStepItemDto>> ToPageAsync(GenStepFilterDto filter)
    {
        Queryable = Queryable
            .WhereNotNull(filter.GenStepType, q => q.GenStepType == filter.GenStepType)
            .WhereNotNull(filter.ProjectId, q => q.ProjectId == filter.ProjectId);
        
        return await ToPageAsync<GenStepFilterDto,GenStepItemDto>(filter);
    }

    /// <summary>
    /// 获取实体详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<GenStepDetailDto?> GetDetailAsync(Guid id)
    {
        return await FindAsync<GenStepDetailDto>(e => e.Id == id);
    }

    /// <summary>
    /// TODO:唯一性判断
    /// </summary>
    /// <param name="unique">唯一标识</param>
    /// <param name="id">排除当前</param>
    /// <returns></returns>
    public async Task<bool> IsUniqueAsync(string unique, Guid? id = null)
    {
        // 自定义唯一性验证参数和逻辑
        return await Command.Where(q => q.Id.ToString() == unique)
            .WhereNotNull(id, q => q.Id != id)
            .AnyAsync();
    }

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="softDelete"></param>
    /// <returns></returns>
    public new async Task<bool?> DeleteAsync(List<Guid> ids, bool softDelete = true)
    {
        return await base.DeleteAsync(ids, softDelete);
    }

    /// <summary>
    /// 数据权限验证
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<GenStep?> GetOwnedAsync(Guid id)
    {
        var query = Command.Where(q => q.Id == id);
        // TODO:自定义数据权限验证
        // query = query.Where(q => q.User.Id == _userContext.UserId);
        return await query.FirstOrDefaultAsync();
    }
}