
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using TuanZi;
using TuanZi.Exceptions;
using TuanZi.Extensions;
using TuanZi.Properties;
using TuanZi.Reflection;


namespace TuanZi.Filter
{
    public static class FilterHelper
    {
        #region Fields

        private static readonly Dictionary<FilterOperate, Func<Expression, Expression, Expression>> ExpressionDict =
            new Dictionary<FilterOperate, Func<Expression, Expression, Expression>>
            {
                {
                    FilterOperate.Equal, Expression.Equal
                },
                {
                    FilterOperate.NotEqual, Expression.NotEqual
                },
                {
                    FilterOperate.Less, Expression.LessThan
                },
                {
                    FilterOperate.Greater, Expression.GreaterThan
                },
                {
                    FilterOperate.LessOrEqual, Expression.LessThanOrEqual
                },
                {
                    FilterOperate.GreaterOrEqual, Expression.GreaterThanOrEqual
                },
                {
                    FilterOperate.StartsWith,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("'StartsWith' {0}' comparison mode only supports type of string");
                        }
                        return Expression.Call(left, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), right);
                    }
                },
                {
                    FilterOperate.EndsWith,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("'EndsWith' {0}' comparison mode only supports type of string");
                        }
                        return Expression.Call(left, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), right);
                    }
                },
                {
                    FilterOperate.Contains,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("'Contains' {0}' comparison mode only supports type of string");
                        }
                        return Expression.Call(left, typeof(string).GetMethod("Contains", new[] { typeof(string) }), right);
                    }
                },
                {
                    FilterOperate.NotContains,
                    (left, right) =>
                    {
                        if (left.Type != typeof(string))
                        {
                            throw new NotSupportedException("'NotContains' {0}' comparison mode only supports type of string");
                        }
                        return Expression.Not(Expression.Call(left, typeof(string).GetMethod("Contains", new[] { typeof(string) }), right));
                    }
                }
            };

        #endregion

        public static Expression<Func<T, bool>> GetExpression<T>(FilterGroup group)
        {
            group.CheckNotNull("group");
            ParameterExpression param = Expression.Parameter(typeof(T), "m");
            Expression body = GetExpressionBody(param, group);
            Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(body, param);
            return expression;
        }

        public static Expression<Func<T, bool>> GetExpression<T>(FilterRule rule)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), "m");
            Expression body = GetExpressionBody(param, rule);
            Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(body, param);
            return expression;
        }

        public static string ToOperateCode(this FilterOperate operate)
        {
            Type type = operate.GetType();
            MemberInfo[] members = type.GetMember(operate.CastTo<string>());
            if (members.Length > 0)
            {
                OperateCodeAttribute attribute = members[0].GetAttribute<OperateCodeAttribute>();
                return attribute == null ? null : attribute.Code;
            }
            return null;
        }

        public static FilterOperate GetFilterOperate(string code)
        {
            code.CheckNotNullOrEmpty("code");
            Type type = typeof(FilterOperate);
            MemberInfo[] members = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
            foreach (MemberInfo member in members)
            {
                FilterOperate operate = member.Name.CastTo<FilterOperate>();
                if (operate.ToOperateCode() == code)
                {
                    return operate;
                }
            }
            throw new NotSupportedException("The query operation to get the opcode enum representation does not support code:" + code);
        }

        #region Private Methods

        private static Expression GetExpressionBody(ParameterExpression param, FilterGroup group)
        {
            param.CheckNotNull("param");

            if (group == null || (group.Rules.Count == 0 && group.Groups.Count == 0))
            {
                return Expression.Constant(true);
            }
            List<Expression> bodys = new List<Expression>();
            bodys.AddRange(group.Rules.Select(rule => GetExpressionBody(param, rule)));
            bodys.AddRange(group.Groups.Select(subGroup => GetExpressionBody(param, subGroup)));

            if (group.Operate == FilterOperate.And)
            {
                return bodys.Aggregate(Expression.AndAlso);
            }
            if (group.Operate == FilterOperate.Or)
            {
                return bodys.Aggregate(Expression.OrElse);
            }
            throw new TuanException(Resources.Filter_GroupOperateError);
        }

        private static Expression GetExpressionBody(ParameterExpression param, FilterRule rule)
        {
            if (rule == null || rule.Value == null || string.IsNullOrEmpty(rule.Value.ToString()))
            {
                return Expression.Constant(true);
            }
            LambdaExpression expression = GetPropertyLambdaExpression(param, rule);
            Expression constant = ChangeTypeToExpression(rule, expression.Body.Type);
            return ExpressionDict[rule.Operate](expression.Body, constant);
        }

        private static LambdaExpression GetPropertyLambdaExpression(ParameterExpression param, FilterRule rule)
        {
            string[] propertyNames = rule.Field.Split('.');
            Expression propertyAccess = param;
            Type type = param.Type;
            foreach (string propertyName in propertyNames)
            {
                PropertyInfo property = type.GetProperty(propertyName);
                if (property == null)
                {
                    throw new InvalidOperationException(string.Format(Resources.Filter_RuleFieldInTypeNotFound, rule.Field, type.FullName));
                }
                type = property.PropertyType;
                propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
            }
            return Expression.Lambda(propertyAccess, param);
        }

        private static Expression ChangeTypeToExpression(FilterRule rule, Type conversionType)
        {
            Type elementType = conversionType.GetUnNullableType();
            object value = rule.Value.CastTo(conversionType);
            return Expression.Constant(value, conversionType);
        }

        #endregion
    }
}
