using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

namespace Netnr;

/// <summary>
/// 谓词构建
/// https://petemontgomery.wordpress.com/2011/02/10/a-universal-predicatebuilder/
/// </summary>
public static class PredicateTo
{
    /// <summary>
    /// Creates a predicate that evaluates to true.
    /// </summary>
    public static Expression<Func<T, bool>> True<T>() { return param => true; }

    /// <summary>
    /// Creates a predicate that evaluates to false.
    /// </summary>
    public static Expression<Func<T, bool>> False<T>() { return param => false; }

    /// <summary>
    /// Creates a predicate expression from the specified lambda expression.
    /// </summary>
    public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) { return predicate; }

    /// <summary>
    /// Combines the first predicate with the second using the logical "and".
    /// </summary>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.AndAlso);
    }

    /// <summary>
    /// Combines the first predicate with the second using the logical "or".
    /// </summary>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.OrElse);
    }

    /// <summary>
    /// Negates the predicate.
    /// </summary>
    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
    {
        var negated = Expression.Not(expression.Body);
        return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
    }

    /// <summary>
    /// Combines the first expression with the second using the specified merge function.
    /// </summary>
    static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
    {
        // zip parameters (map from parameters of second to parameters of first)
        var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

        // replace parameters in the second lambda expression with the parameters in the first
        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

        // create a merged lambda expression with parameters from the first expression
        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }

    class ParameterRebinder : ExpressionVisitor
    {
        readonly Dictionary<ParameterExpression, ParameterExpression> map;

        ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (map.TryGetValue(p, out ParameterExpression replacement))
            {
                p = replacement;
            }

            return base.VisitParameter(p);
        }
    }

    /// <summary>
    /// 查询列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TField">字段类型</typeparam>
    /// <param name="propertyName">字段名</param>
    /// <returns></returns>
    public static Expression<Func<T, TField>> Field<T, TField>(string propertyName)
    {
        var parameterExp = Expression.Parameter(typeof(T), "x");
        var propertyExp = Expression.Property(parameterExp, propertyName);
        var finalExp = Expression.Lambda<Func<T, TField>>(propertyExp, parameterExp);
        return finalExp;
    }

    /// <summary>
    /// 比较
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName">字段名</param>
    /// <param name="comparator">比较符</param>
    /// <param name="propertyValue">值</param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> Compare<T>(string propertyName, string comparator, object propertyValue)
    {
        var ttype = typeof(T);

        var propertyType = ttype.GetProperty(propertyName).PropertyType;
        var value = propertyValue.ToString().ToConvert(propertyType);

        var parameterExp = Expression.Parameter(ttype, "x");
        var propertyExp = Expression.Property(parameterExp, propertyName); //x.Field
        var someValue = Expression.Constant(value, propertyType); //value
        BinaryExpression body = comparator switch
        {
            "=" or "Equal" => Expression.Equal(propertyExp, someValue),
            "!=" or "NotEqual" => Expression.NotEqual(propertyExp, someValue),
            ">" or "GreaterThan" => Expression.GreaterThan(propertyExp, someValue),
            ">=" or "GreaterThanOrEqual" => Expression.GreaterThanOrEqual(propertyExp, someValue),//x.Field >= value
            "<" or "LessThan" => Expression.LessThan(propertyExp, someValue),
            "<=" or "LessThanOrEqual" => Expression.LessThanOrEqual(propertyExp, someValue),
            _ => throw new Exception($"not support {comparator}"),
        };
        var finalExp = Expression.Lambda<Func<T, bool>>(body, parameterExp); // x => x.Field >= value
        return finalExp;
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyName">字段名</param>
    /// <param name="containment">包含符 Contains、StartsWith、EndsWith</param>
    /// <param name="propertyValue">值</param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> Contains<T>(string propertyName, string containment, string propertyValue)
    {
        var parameterExp = Expression.Parameter(typeof(T), "x");
        var propertyExp = Expression.Property(parameterExp, propertyName);
        MethodInfo method = typeof(string).GetMethod(containment, new[] { typeof(string) });
        var someValue = Expression.Constant(propertyValue, typeof(string));
        var containsMethodExp = Expression.Call(propertyExp, method, someValue);

        var finalExp = Expression.Lambda<Func<T, bool>>(containsMethodExp, parameterExp);
        return finalExp;
    }
}
