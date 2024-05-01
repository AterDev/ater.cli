using Definition.Entity;
namespace Definition.Share.Models.ApiDocInfoDtos;
/// <summary>
/// 接口文档查询筛选
/// </summary>
/// <see cref="Definition.Entity.ApiDocInfo"/>
public class ApiDocInfoFilterDto : FilterBase
{
    /// <summary>
    /// 文档名称
    /// </summary>
    [MaxLength(100)]
    public string? Name { get; set; }
    public Guid? ProjectId { get; set; }
}