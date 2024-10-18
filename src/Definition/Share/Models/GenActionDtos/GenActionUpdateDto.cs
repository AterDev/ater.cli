﻿namespace Share.Models.GenActionDtos;
/// <summary>
/// The project's generate action更新时DTO
/// </summary>
/// <see cref="Entity.GenAction"/>
public class GenActionUpdateDto
{
    /// <summary>
    /// action name
    /// </summary>
    [MaxLength(40)]
    public string? Name { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; }
    /// <summary>
    /// source type
    /// </summary>
    public GenSourceType? SourceType { get; set; }
}
