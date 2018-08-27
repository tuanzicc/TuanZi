
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using TuanZi;
using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Exceptions;
using TuanZi.Extensions;
using TuanZi.Properties;
using TuanZi.Reflection;
using TuanZi.Secutiry;
using TuanZi.Secutiry.Claims;

namespace TuanZi.Filter
{
    
    public static class FilterHelper
    {
        #region Fields.8742

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
                        return Expression.Call(left,
                            typeof(string).GetMethod("StartsWith", new[] { typeof(string) })
                            ?? throw new InvalidOperationException($"The method named 'StartsWith' does not exist"),
                            right);
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
                        return Expression.Call(left,
                            typeof(string).GetMethod("EndsWith", new[] { typeof(string) })
                            ?? throw new InvalidOperationException($"The method named 'EndsWith' does not exist"),
                            right);
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
                        return Expression.Call(left,
                            typeof(string).GetMethod("Contains", new[] { typeof(string) })
                            ?? throw new InvalidOperationException($"The method named 'Contains' does not exist"),
                            right);
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
                        return Expression.Not(Expression.Call(left,
                            typeof(string).GetMethod("Contains", new[] { typeof(string) })
                            ?? throw new InvalidOperationException($"The method named 'NotContains' does not exist"),
                            right));
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

        public static Expression<Func<T, bool>> GetDataFilterExpression<T>(FilterGroup group = null,
            DataAuthOperation operation = DataAuthOperation.Read)
        {
            Expression<Func<T, bool>> exp = m => true;
            if (group != null)
            {
                exp = GetExpression<T>(group);
            }
            ClaimsPrincipal user = ServiceLocator.Instance.GetCurrentUser();
            if (user == null)
            {
                return exp;
            }

            IDataAuthCache dataAuthCache = ServiceLocator.Instance.GetService<IDataAuthCache>();
            if (dataAuthCache == null)
            {
                return exp;
            }

            string[] roleNames = user.Identity.GetRoles();
            ScopedDictionary scopedDict = ServiceLocator.Instance.GetService<ScopedDictionary>();
            if (scopedDict?.Function != null)
            {
                roleNames = scopedDict.DataAuthValidRoleNames;
            }
            string typeName = typeof(T).GetFullNameWithModule();
            Expression<Func<T, bool>> subExp = null;
            foreach (string roleName in roleNames)
            {
                FilterGroup subGroup = dataAuthCache.GetFilterGroup(roleName, typeName, operation);
                if (subGroup == null)
                {
                    continue;
                }
                subExp = subExp == null ? GetExpression<T>(subGroup) : subExp.Or(GetExpression<T>(subGroup));
            }
            if (subExp != null)
            {
                if (group == null)
                {
                    return subExp;
                }
                exp = subExp.And(exp);
            }

            return exp;
        }

        public static Expression<Func<T, bool>> GetExpression<T>(FilterRule rule)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), "m");
            Expression body = GetExpressionBody(param, rule);
            Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(body, param);
            return expression;
        }

        public static OperationResult CheckFilterGroup(FilterGroup group, Type type)
        {
            try
            {
                ParameterExpression param = Expression.Parameter(type, "m");
                GetExpressionBody(param, group);
                return OperationResult.Success;
            }
            catch (Exception ex)
            {
                return new OperationResult(OperationResultType.Error, $"Conditional group validation failed:{ex.Message}");
            }
        }

        public static string ToOperateCode(this FilterOperate operate)
        {
            Type type = operate.GetType();
            MemberInfo[] members = type.GetMember(operate.CastTo<string>());
            if (members.Length == 0)
            {
                return null;
            }

            OperateCodeAttribute attribute = members[0].GetAttribute<OperateCodeAttribute>();
            return attribute?.Code;
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
            throw new NotSupportedException("The query operation that gets the opcode does not support the code when it is enumerated:" + code);
        }

        #region Privates

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
            if (rule.Value?.ToString() == "@CurrentUserId")
            {
                if (rule.Operate != FilterOperate.Equal)
                {
                    throw new TuanException($"The current user '{rule.Value}' can only be used in the '{FilterOperate.Equal.ToDescription()}' operation");
                }
                ClaimsPrincipal user = ServiceLocator.Instance.GetCurrentUser();
                if (user == null || !user.Identity.IsAuthenticated)
                {
                    throw new TuanException("Need to get the current user number, but the current user is empty, may not be logged in or has expired");
                }
                object value = user.Identity.GetClaimValueFirstOrDefault(ClaimTypes.NameIdentifier);
                value = value.CastTo(conversionType);
                return Expression.Constant(value, conversionType);
            }
            else
            {
                object value = rule.Value.CastTo(conversionType);
                return Expression.Constant(value, conversionType);
            }
        }

        #endregion
    }
}
