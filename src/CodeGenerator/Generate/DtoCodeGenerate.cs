﻿using static CodeGenerator.Models.EntityInfo;
using PropertyInfo = CodeGenerator.Models.PropertyInfo;

namespace CodeGenerator.Generate;
/// <summary>
/// dto generate
/// </summary>
public class DtoCodeGenerate : GenerateBase
{
    public EntityInfo? EntityInfo { get; set; }
    public string KeyType { get; set; } = "Guid";
    /// <summary>
    /// dto 输出的 程序集名称
    /// </summary>
    public string AssemblyName { get; set; } = "Share";
    public DtoCodeGenerate(string entityPath)
    {
        if (File.Exists(entityPath))
        {
            var entityHelper = new EntityParseHelper(entityPath);
            EntityInfo = entityHelper.GetEntity();
            KeyType = (EntityInfo.KeyType) switch
            {
                EntityKeyType.Int => "Int",
                EntityKeyType.String => "String",
                _ => "Guid"
            };
        }
        else
        {
            _ = new FileNotFoundException();
        }
    }

    public string? GetShortDto()
    {
        if (EntityInfo == null) return default;
        var dto = new DtoInfo
        {
            Name = EntityInfo.Name + "ShortDto",
            NamespaceName = EntityInfo.NamespaceName,
            Comment = EntityInfo.Comment,
            Tag = EntityInfo.Name,
            Properties = EntityInfo.PropertyInfos?
                .Where(p => p.Name != "Content"
                    && (p.MaxLength < 2000 || p.MaxLength == null)
                    && !(p.IsList && p.IsNavigation))
                .ToList()
        };
        return dto.ToString(AssemblyName, EntityInfo.Name);
    }

    public string? GetItemDto()
    {
        if (EntityInfo == null) return default;
        var dto = new DtoInfo
        {
            Name = EntityInfo.Name + "ItemDto",
            NamespaceName = EntityInfo.NamespaceName,
            Comment = EntityInfo.Comment,
            Tag = EntityInfo.Name,
            Properties = EntityInfo.PropertyInfos?
                .Where(p => !p.IsList
                    && p.Name != "Content"
                    && (p.MaxLength < 1000 || p.MaxLength == null)
                    && !p.IsNavigation)
                .ToList()
        };
        return dto.ToString(AssemblyName, EntityInfo.Name);
    }

    public string? GetFilterDto()
    {
        if (EntityInfo == null) return default;
        var referenceProps = EntityInfo.PropertyInfos?
            .Where(p => p.IsNavigation && !p.IsList)
            .Select(s => new PropertyInfo($"{KeyType}?", s.Name + "Id"))
            .ToList();

        var filterFields = new string[] { "Id", "CreatedTime", "UpdatedTime" };
        var dto = new DtoInfo
        {
            Name = EntityInfo.Name + "Filter",
            NamespaceName = EntityInfo.NamespaceName,
            Comment = EntityInfo.Comment,
            Tag = EntityInfo.Name,
            BaseType = "FilterBase",
            Properties = EntityInfo.PropertyInfos?
                .Where(p => p.IsRequired
                    || (!p.IsNullable
                        && !p.IsList
                        && !p.IsNavigation
                        && !filterFields.Contains(p.Name))
                    )
                .ToList()
        };
        referenceProps?.ForEach(item =>
        {
            dto.Properties?.Add(item);
        });
        return dto.ToString(AssemblyName, EntityInfo.Name);
    }

    public string? GetAddDto()
    {
        if (EntityInfo == null) return default;
        var referenceProps = EntityInfo.PropertyInfos?
            .Where(p => p.IsNavigation && !p.IsList)
            .Select(s => new PropertyInfo($"{KeyType}", s.Name + "Id"))
            .ToList();
        var dto = new DtoInfo
        {
            Name = EntityInfo.Name + "AddDto",
            NamespaceName = EntityInfo.NamespaceName,
            Comment = EntityInfo.Comment,
            Tag = EntityInfo.Name,
            Properties = EntityInfo.PropertyInfos?.Where(p => p.Name != "Id"
                && p.Name != "CreatedTime"
                && p.Name != "UpdatedTime"
                && !p.IsList
                && !p.IsNavigation)
            .ToList()
        };
        referenceProps?.ForEach(item =>
        {
            dto.Properties?.Add(item);
        });
        return dto.ToString(AssemblyName, EntityInfo.Name);
    }

    /// <summary>
    /// 更新dto
    /// 导航属性Name+Id,过滤列表属性
    /// </summary>
    /// <returns></returns>
    public string? GetUpdateDto()
    {
        if (EntityInfo == null) return default;
        // 导航属性处理
        var referenceProps = EntityInfo.PropertyInfos?
            .Where(p => p.IsNavigation && !p.IsList && !p.IsNullable)
            .Select(s => new PropertyInfo($"{KeyType}?", s.Name + "Id"))
            .ToList();
        var dto = new DtoInfo
        {
            Name = EntityInfo.Name + "UpdateDto",
            NamespaceName = EntityInfo.NamespaceName,
            Comment = EntityInfo.Comment,
            Tag = EntityInfo.Name,
            Properties = EntityInfo.PropertyInfos?.Where(p => p.Name != "Id"
                && p.Name != "CreatedTime"
                && p.Name != "UpdatedTime"
                && !p.IsList
                && !p.IsNavigation).ToList()
        };
        // 处理非required的都设置为nullable
        if (dto.Properties != null)
            foreach (var item in dto.Properties)
            {
                if (!item.IsRequired)
                {
                    item.IsNullable = true;
                }
            }
        referenceProps?.ForEach(item =>
        {
            dto.Properties?.Add(item);
        });
        return dto.ToString(AssemblyName, EntityInfo.Name);
    }

    public string GetDtoUsings()
    {
        var nsName = EntityInfo!.NamespaceName;
        return @$"global using System.ComponentModel.DataAnnotations;
global using {AssemblyName}.Models;
global using {nsName};";
    }
    public string GetFilterBase()
    {
        return @$"namespace {AssemblyName}.Models;
public class FilterBase
{{
    public int? PageIndex {{ get; set; }} = 1;
    public int? PageSize {{ get; set; }} = 12;
    public {KeyType}? TenantId {{ get; set; }}
    public DateTimeOffset? MinCreatedTime {{ get; set; }}
    public DateTimeOffset? MaxCreatedTime {{ get; set; }}
}}
";
    }

    public string GetPageResult()
    {
        var content = GetTplContent("PageResult.tpl");
        if (content.NotNull())
        {
            content = content.Replace("@{AssemblyName}", AssemblyName);
        }
        return content;
    }

    /// <summary>
    /// 生成AutoMapperProfile
    /// </summary>
    /// <param name="entityName"></param>
    protected void GenerateAutoMapperProfile(string entityName)
    {
        var code =
    @$"            CreateMap<{entityName}AddDto, {entityName}>();
            CreateMap<{entityName}UpdateDto, {entityName}>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => NotNull(srcMember)));;
            CreateMap<{entityName}, {entityName}Dto>();
            CreateMap<{entityName}, {entityName}ItemDto>();
            CreateMap<{entityName}, {entityName}DetailDto>();        
";
        // 先判断是否存在配置文件
        var path = Path.Combine("", "AutoMapper");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        const string AppendSign = "// {AppendMappers}";
        const string AlreadySign = "// {AlreadyMapedEntity}";
        var mapperFilePath = Path.Combine(path, "AutoGenerateProfile.cs");
        string content;
        if (File.Exists(mapperFilePath))
        {
            // 如果文件存在但当前entity没有生成mapper，则替换该文件
            content = File.ReadAllText(mapperFilePath);
            if (!content.Contains($"// {entityName};"))
            {
                Console.WriteLine("添加Mapper：" + entityName);
                content = content.Replace(AlreadySign, $"// {entityName};\r\n" + AlreadySign);
                content = content.Replace(AppendSign, code + AppendSign);
            }
            else
            {
                Console.WriteLine("已存在:" + entityName);
            }
        }
        else
        {
            // 读取模板文件
            content = GetTplContent("AutoMapper.tpl");
            content = content.Replace(AppendSign, code + AppendSign);
        }
        // 写入文件
        File.WriteAllText(mapperFilePath, content, Encoding.UTF8);
        Console.WriteLine("AutoMapper 配置完成");
    }
}