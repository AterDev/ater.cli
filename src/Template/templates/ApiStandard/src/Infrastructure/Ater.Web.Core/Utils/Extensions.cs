using System.Data;
using System.Diagnostics.CodeAnalysis;
using Ater.Web.Core.Models;
using Ater.Web.Core.Utils;

using Mapster;

namespace Ater.Web.Core.Utils;
public static partial class Extensions
{
    /// <summary>
    /// 将被合并对象中非空属性值，合并到源对象
    /// </summary>
    /// <typeparam name="TSource">源对象</typeparam>
    /// <typeparam name="TMerge">被合并对象</typeparam>
    /// <param name="source"></param>
    /// <param name="merge"></param>
    /// <param name="ignoreNull">是否忽略 null</param>
    /// <returns></returns>
    public static TSource Merge<TSource, TMerge>(this TSource source, TMerge merge, bool ignoreNull = true)
    {
        if (ignoreNull)
        {
            _ = TypeAdapterConfig<TMerge, TSource>
                .NewConfig()
                .IgnoreNullValues(true);
        }
        return merge.Adapt(source);
    }

    /// <summary>
    /// 类型转换
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TDestination">目标类型</typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static TDestination MapTo<TSource, TDestination>(this TSource source) where TDestination : class
    {
        _ = TypeAdapterConfig<TSource, TDestination>
           .NewConfig()
           .IgnoreNullValues(true);
        return source.Adapt<TSource, TDestination>();
    }

    /// <summary>
    /// 构造查询Dto
    /// 重要: dto中属性名称和类型必须与实体一致
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public static IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source)
    {
        Type sourceType = typeof(TSource);
        Type resultType = typeof(TResult);
        ParameterExpression parameter = Expression.Parameter(sourceType, "e");

        // 只构造都存在的属性
        var sourceNames = sourceType.GetProperties()
            .Select(s => s.Name).ToList();
        var props = resultType.GetProperties().ToList();
        props = props.Where(p => sourceNames.Contains(p.Name)).ToList();
        //props = props.Intersect(sourceProps).ToList();

        var bindings = props.Select(p =>
             Expression.Bind(p, Expression.PropertyOrField(parameter, p.Name))
        ).ToList();
        MemberInitExpression body = Expression.MemberInit(Expression.New(resultType), bindings);
        LambdaExpression selector = Expression.Lambda(body, parameter);
        return source.Provider.CreateQuery<TResult>(
            Expression.Call(typeof(Queryable), "Select", [sourceType, resultType],
                source.Expression, Expression.Quote(selector)));
    }

    /// <summary>
    /// 不为空(字符串)时执行的条件
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="field">要判断的字段</param>
    /// <param name="expression">不为空时执行的条件</param>
    /// <returns></returns>
    public static IQueryable<TSource> WhereNotNull<TSource>(this IQueryable<TSource> source, object? field, Expression<Func<TSource, bool>> expression)
    {
        if (field is not null)
        {
            if (field is string str && str.IsEmpty())
            {
                return source;
            }
            return source.Where(expression);
        }
        return source;
    }
  
    public static IQueryable<TResult> ProjectTo<TResult>(this IQueryable source)
    {
        return source.ProjectToType<TResult>();
    }

    /// <summary>
    /// 排序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="dic"></param>
    /// <returns></returns>
    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, Dictionary<string, bool> dic)
    {
        IOrderedQueryable<T> orderQuery = default!;
        ParameterExpression parameter = Expression.Parameter(typeof(T), "e");
        var count = 0;
        foreach (KeyValuePair<string, bool> item in dic)
        {
            MemberExpression prop = Expression.PropertyOrField(parameter, item.Key);
            MemberExpression body = Expression.MakeMemberAccess(parameter, prop.Member);
            LambdaExpression selector = Expression.Lambda(body, parameter);
            MethodCallExpression expression = count > 0
                ? item.Value
                    ? Expression.Call(typeof(Queryable),
                                              "ThenBy",
                                              [typeof(T), body.Type],
                                              query.Expression,
                                              Expression.Quote(selector))
                    : Expression.Call(typeof(Queryable),
                                              "ThenByDescending",
                                              [typeof(T), body.Type],
                                              query.Expression,
                                              Expression.Quote(selector))
                : item.Value
                    ? Expression.Call(typeof(Queryable),
                                              "OrderBy",
                                              [typeof(T), body.Type],
                                              query.Expression,
                                              Expression.Quote(selector))
                    : Expression.Call(typeof(Queryable),
                                              "OrderByDescending",
                                              [typeof(T), body.Type],
                                              query.Expression,
                                              Expression.Quote(selector));
            orderQuery = (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(expression);
            query = orderQuery;
            count++;
        }
        return orderQuery;
    }


    public static IOrderedQueryable<T> ThenBy<T>(this IQueryable<T> query, Dictionary<string, bool> dic)
    {
        IOrderedQueryable<T> orderQuery = default!;
        ParameterExpression parameter = Expression.Parameter(typeof(T), "e");
        foreach (KeyValuePair<string, bool> item in dic)
        {
            MemberExpression prop = Expression.PropertyOrField(parameter, item.Key);
            MemberExpression body = Expression.MakeMemberAccess(parameter, prop.Member);
            LambdaExpression selector = Expression.Lambda(body, parameter);
            MethodCallExpression expression = item.Value
                    ? Expression.Call(typeof(Queryable),
                                              "ThenBy",
                                              [typeof(T), body.Type],
                                              query.Expression,
                                              Expression.Quote(selector))
                    : Expression.Call(typeof(Queryable),
                                              "ThenByDescending",
                                              [typeof(T), body.Type],
                                              query.Expression,
                                              Expression.Quote(selector));

            orderQuery = (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(expression);
            query = orderQuery;
        }
        return orderQuery;
    }

    /// <summary>
    /// 构建Between表达式
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="propertyExpression"></param>
    /// <param name="minVal"></param>
    /// <param name="maxVal"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static IQueryable<TSource> Between<TSource, TValue>(
        this IQueryable<TSource> source,
        Expression<Func<TSource, TValue>> propertyExpression,
        TValue minVal,
        TValue maxVal)
        where TValue : struct
    {
        ParameterExpression parameter = propertyExpression.Parameters[0];
        MemberExpression? memberExpression = (propertyExpression.Body as MemberExpression
            ?? (propertyExpression.Body as UnaryExpression)?.Operand as MemberExpression)
            ?? throw new ArgumentException("Invalid property expression", nameof(propertyExpression));
        Type propertyType = memberExpression.Type;

        var minValObj = Convert.ChangeType(minVal, propertyType);
        var maxValObj = Convert.ChangeType(maxVal, propertyType);

        ConstantExpression minExpr = Expression.Constant(minValObj, propertyType);
        ConstantExpression maxExpr = Expression.Constant(maxValObj, propertyType);

        MemberExpression propertyAccess = Expression.MakeMemberAccess(parameter, memberExpression.Member);
        BinaryExpression leftExpr = Expression.GreaterThanOrEqual(propertyAccess, minExpr);
        BinaryExpression rightExpr = Expression.LessThanOrEqual(propertyAccess, maxExpr);
        BinaryExpression andExpr = Expression.AndAlso(leftExpr, rightExpr);
        var lambdaExpr = Expression.Lambda<Func<TSource, bool>>(andExpr, parameter);

        return source.Where(lambdaExpr);
    }

    /// <summary>
    /// 范围查询:int
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="propertyExpression"></param>
    /// <param name="minVal"></param>
    /// <param name="maxVal"></param>
    /// <returns></returns>
    public static IQueryable<T> Between<T>(
        this IQueryable<T> source,
        Expression<Func<T, int>> propertyExpression,
        int minVal,
        int maxVal)
    {
        return source.Between<T, int>(propertyExpression, minVal, maxVal);
    }
    /// <summary>
    /// 范围查询:long
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="propertyExpression"></param>
    /// <param name="minVal"></param>
    /// <param name="maxVal"></param>
    /// <returns></returns>
    public static IQueryable<T> Between<T>(
        this IQueryable<T> source,
        Expression<Func<T, long>> propertyExpression,
        int minVal,
        int maxVal)
    {
        return source.Between<T, long>(propertyExpression, minVal, maxVal);
    }

    /// <summary>
    /// 范围查询:double
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="propertyExpression"></param>
    /// <param name="minVal"></param>
    /// <param name="maxVal"></param>
    /// <returns></returns>
    public static IQueryable<T> Between<T>(
        this IQueryable<T> source,
        Expression<Func<T, double>> propertyExpression,
        int minVal,
        int maxVal)
    {
        return source.Between<T, double>(propertyExpression, minVal, maxVal);
    }

    /// <summary>
    /// 范围条件:DateOnly
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="propertyExpression"></param>
    /// <param name="minVal"></param>
    /// <param name="maxVal"></param>
    /// <returns></returns>
    public static IQueryable<T> Between<T>(
        this IQueryable<T> source,
        Expression<Func<T, DateTimeOffset>> propertyExpression,
        DateOnly minVal,
        DateOnly maxVal)
    {
        var minValObj = minVal.ToDateTimeOffset();
        var maxValObj = maxVal.ToDateTimeOffset();
        return source.Between<T, DateTimeOffset>(propertyExpression, minValObj, maxValObj);
    }

    /// <summary>
    /// 范围查询:DateTimeOffset
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="propertyExpression"></param>
    /// <param name="minVal"></param>
    /// <param name="maxVal"></param>
    /// <returns></returns>
    public static IQueryable<T> Between<T>(
        this IQueryable<T> source,
        Expression<Func<T, DateTimeOffset>> propertyExpression,
        DateTimeOffset minVal,
        DateTimeOffset maxVal)
    {
        return source.Between<T, DateTimeOffset>(propertyExpression, minVal, maxVal);
    }

    /// <summary>
    /// 带有根节点的列表转成树型结构
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="nodes"></param>
    /// <returns></returns>
    public static List<T> BuildTree<T>(this List<T> nodes) where T : ITreeNode<T>
    {
        nodes.ForEach(n => { n.Children = []; });
        var nodeDict = nodes.ToDictionary(n => n.Id);
        List<T> res = [];

        foreach (T node in nodes)
        {
            if (node.ParentId == null)
            {
                res.Add(node);
            }
            else
            {
                if (nodeDict.TryGetValue(node.ParentId.Value, out T? value))
                {
                    value.Children ??= [];
                    value.Children!.Add(node);
                }
            }
        }
        return res;
    }
}
