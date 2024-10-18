﻿using Application.Managers;
using Microsoft.AspNetCore.Mvc;

namespace AterStudio.Controllers;

/// <summary>
/// 项目
/// </summary>
/// <see cref="ProjectManager"/>
public class ProjectController(
    ProjectManager manager,
    IProjectContext project,
    AdvanceManager advance,
    ILogger<ProjectContext> logger) : BaseController<ProjectManager>(manager, project, logger)
{
    private readonly AdvanceManager _advance = advance;

    /// <summary>
    /// 获取解决方案列表
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<Project>> ListAsync()
    {
        return await _manager.ListAsync();
    }

    /// <summary>
    /// 获取工具版本
    /// </summary>
    /// <returns></returns>
    [HttpGet("version")]
    public string GetVersion()
    {
        return _manager.GetToolVersion();
    }

    /// <summary>
    /// 详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<Project?> ProjectAsync([FromRoute] Guid id)
    {
        return await _manager.GetDetailAsync(id);
    }

    /// <summary>
    /// 添加项目
    /// </summary>
    /// <param name="name"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<Guid?>> AddAsync(string name, string path)
    {
        if (!Directory.Exists(path))
        {
            return Problem("未找到该目录");
        }

        var projectFilePath = Directory.GetFiles(path, $"*{ConstVal.SolutionExtension}", SearchOption.TopDirectoryOnly).FirstOrDefault();

        projectFilePath ??= Directory.GetFiles(path, $"*{ConstVal.SolutionXMLExtension}", SearchOption.TopDirectoryOnly).FirstOrDefault();

        projectFilePath ??= Directory.GetFiles(path, $"*{ConstVal.CSharpProjectExtension}", SearchOption.TopDirectoryOnly).FirstOrDefault();
        projectFilePath ??= Directory.GetFiles(path, ConstVal.NodeProjectFile, SearchOption.TopDirectoryOnly).FirstOrDefault();

        if (projectFilePath == null)
        {
            return Problem("Not Found valid Project!");
        }
        return await _manager.AddAsync(name, projectFilePath);
    }

    /// <summary>
    /// 添加微服务
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpPost("service")]
    public ActionResult<bool> AddService(string name)
    {
        // TODO: AddServiceProject
        //bool res = _manager.AddServiceProject(name);
        return false;
    }

    /// <summary>
    /// 更新解决方案
    /// </summary>
    /// <returns></returns>
    [HttpPut("solution")]
    public async Task<string> UpdateSolutionAsync()
    {
        return await _manager.UpdateSolutionAsync();
    }

    /// <summary>
    /// 打开解决方案，仅支持sln
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    [HttpPost("open")]
    public ActionResult<string> OpenSolution(string path)
    {
        return path.EndsWith(".sln") ? _manager.OpenSolution(path) : Problem("不支持的解决方案文件");
    }

    /// <summary>
    /// 更新配置
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("setting/{id}")]
    public async Task<ActionResult<bool>> UpdateConfigAsync([FromRoute] Guid id, ProjectConfig dto)
    {
        var project = await _manager.GetCurrentAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        return await _manager.UpdateConfigAsync(project, dto);
    }

    /// <summary>
    /// 删除项目
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteAsync([FromRoute] Guid id)
    {
        return await _manager.DeleteAsync([id], false);
    }
}
