﻿namespace CodeGenerator.Models;

/// <summary>
/// 控制器模板模型
/// </summary>
public class ControllerViewModel
{
    public string? Namespace { get; set; }
    public string? EntityName { get; set; }
    public string? ShareNamespace { get; set; }
}
