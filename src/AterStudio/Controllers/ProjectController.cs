﻿using AterStudio.Manager;
using AterStudio.Models;
using Core.Infrastructure;
using Datastore;
using Microsoft.AspNetCore.Mvc;

namespace AterStudio.Controllers;

/// <summary>
/// 项目
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly ProjectManager _manager;
    public ProjectController(ProjectManager manager)
    {
        _manager = manager;
    }

    [HttpGet]
    public List<Project> List()
    {
        return _manager.GetProjects();
    }

    /// <summary>
    /// 详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public Project? Project([FromRoute] Guid id)
    {
        return _manager.GetProject(id);
    }

    [HttpGet("sub/{id}")]
    public List<SubProjectInfo>? GetAllProjectInfos([FromRoute] Guid id)
    {
        return _manager.GetAllProjects(id);
    }

    [HttpPost]
    public async Task<ActionResult<Project?>> AddAsync(string name, string path)
    {
        return (!System.IO.File.Exists(path) && !Directory.Exists(path))
            ? Problem("未找到该路径")
            : await _manager.AddProjectAsync(name, path);
    }

    /// <summary>
    /// 删除项目
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public ActionResult<bool> Delete([FromRoute] Guid id)
    {
        return _manager.DeleteProject(id);
    }

    /// <summary>
    /// 获取监听状态
    /// </summary>
    /// <returns></returns>
    [HttpGet("watcher/{id}")]
    public ActionResult<bool> GetWatcherStatus([FromRoute] Guid id)
    {
        var project = _manager.GetProject(id);
        if (project == null)
        {
            return NotFound("不存在该项目");
        }
        return _manager.GetWatcherStatus(project);
    }

    /// <summary>
    /// 开启监测
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("watcher/{id}")]
    public ActionResult<bool> StartWatcher([FromRoute] Guid id)
    {
        var project = _manager.GetProject(id);
        if (project == null)
        {
            return NotFound("不存在该项目");
        }

        Const.PROJECT_ID = project.ProjectId;
        _manager.StartWatcher(project);
        return true;
    }

    /// <summary>
    /// 停止监测
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("watcher/{id}")]
    public ActionResult<bool> StopWatcher([FromRoute] Guid id)
    {
        var project = _manager.GetProject(id);
        if (project == null)
        {
            return NotFound("不存在该项目");
        }
        Const.PROJECT_ID = project.ProjectId;
        _manager.StopWatcher(project);
        return true;
    }
}
