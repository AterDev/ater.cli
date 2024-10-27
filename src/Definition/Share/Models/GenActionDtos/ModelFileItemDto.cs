﻿namespace Share.Models.GenActionDtos;
/// <summary>
/// 模型文件项
/// </summary>
public class ModelFileItemDto
{
    /// <summary>
    /// 名称
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// 路径
    /// </summary>
    public required string FullName { get; set; }
}
