﻿namespace Datastore.Models;

/// <summary>
/// 属性变化
/// </summary>
public class PropertyChange
{
    /// <summary>
    /// 属性名称
    /// </summary>
    public required string Name { get; set; }
    public required ChangeType Type { get; set; }
}

public enum ChangeType
{
    Add,
    Update,
    Delete,
}
