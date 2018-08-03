using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TuanZi.Data;
using TuanZi.Extensions;

namespace TuanZi.Reflection
{
    public static class TypeExtensions
    {
        public static bool IsDeriveClassFrom<TBaseType>(this Type type, bool canAbstract = false)
        {
            return IsDeriveClassFrom(type, typeof(TBaseType), canAbstract);
        }

        public static bool IsDeriveClassFrom(this Type type, Type baseType, bool canAbstract = false)
        {
            Check.NotNull(type, nameof(type));
            Check.NotNull(baseType, nameof(baseType));

            return type.IsClass && (!canAbstract && !type.IsAbstract) && type.IsBaseOn(baseType);
        }

        public static bool IsNullableType(this Type type)
        {
            return ((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static Type GetNonNummableType(this Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        public static Type GetUnNullableType(this Type type)
        {
            if (IsNullableType(type))
            {
                NullableConverter nullableConverter = new NullableConverter(type);
                return nullableConverter.UnderlyingType;
            }
            return type;
        }

        public static string GetDescription(this Type type, bool inherit = false)
        {
            DescriptionAttribute desc = type.GetAttribute<DescriptionAttribute>(inherit);
            return desc == null ? type.FullName : desc.Description;
        }

        public static string GetDescription(this MemberInfo member, bool inherit = false)
        {
            DescriptionAttribute desc = member.GetAttribute<DescriptionAttribute>(inherit);
            if (desc != null)
            {
                return desc.Description;
            }
            DisplayNameAttribute displayName = member.GetAttribute<DisplayNameAttribute>(inherit);
            if (displayName != null)
            {
                return displayName.DisplayName;
            }
            DisplayAttribute display = member.GetAttribute<DisplayAttribute>(inherit);
            if (display != null)
            {
                return display.Name;
            }
            return member.Name;
        }

        public static bool HasAttribute<T>(this MemberInfo memberInfo, bool inherit = true) where T : Attribute
        {
            return memberInfo.IsDefined(typeof(T), inherit);
        }

        public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            var descripts = memberInfo.GetCustomAttributes(typeof(T), inherit);
            return descripts.FirstOrDefault() as T;
        }

        public static T[] GetAttributes<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(T), inherit).Cast<T>().ToArray();
        }

        public static bool IsEnumerable(this Type type)
        {
            if (type == typeof(string))
            {
                return false;
            }
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static bool IsGenericAssignableFrom(this Type genericType, Type type)
        {
            genericType.CheckNotNull("genericType");
            type.CheckNotNull("type");
            if (!genericType.IsGenericType)
            {
                throw new ArgumentException("This function only supports generic types of calls. Non-generic types can use the IsAssignableFrom method.");
            }

            List<Type> allOthers = new List<Type> { type };
            if (genericType.IsInterface)
            {
                allOthers.AddRange(type.GetInterfaces());
            }

            foreach (var other in allOthers)
            {
                Type cur = other;
                while (cur != null)
                {
                    if (cur.IsGenericType)
                    {
                        cur = cur.GetGenericTypeDefinition();
                    }
                    if (cur.IsSubclassOf(genericType) || cur == genericType)
                    {
                        return true;
                    }
                    cur = cur.BaseType;
                }
            }
            return false;
        }

        public static bool IsAsync(this MethodInfo method)
        {
            return method.ReturnType == typeof(Task)
                || method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);
        }

        public static bool IsBaseOn(this Type type, Type baseType)
        {
            if (baseType.IsGenericTypeDefinition)
            {
                return baseType.IsGenericAssignableFrom(type);
            }
            return baseType.IsAssignableFrom(type);
        }

        public static bool IsBaseOn<TBaseType>(this Type type)
        {
            Type baseType = typeof(TBaseType);
            return type.IsBaseOn(baseType);
        }

        public static string GetFullNameWithModule(this Type type)
        {
            return $"{type.FullName},{type.Module.Name.Replace(".dll", "").Replace(".exe", "")}";
        }

        public static IEnumerable<PropertyInfo> FindProperties(this Type type, string name, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            PropertyInfo property = null;
            foreach (var propName in name.Split('.'))
            {
                if (property == null)
                {
                    property = type.GetProperty(propName, bindingFlags);
                    ThrowIfPropertyNull(property, name);
                    yield return property;
                    continue;
                }

                property = property.PropertyType.GetProperty(propName, bindingFlags);
                ThrowIfPropertyNull(property, name);
                yield return property;
            }
        }

        public static void ThrowIfPropertyNull(PropertyInfo propertyInfo, string name)
        {
            if (propertyInfo == null)
                throw new ArgumentOutOfRangeException(nameof(name), $"Property name {name} is not valid.");
        }


        public static bool HasProperty(this object obj, string name)
        {
            return obj.GetType().GetProperty(name) != null;

        }

        public static object GetPropertyValue(this object obj, string name)
        {
            var prop = obj.GetType().GetProperty(name);
            if (prop != null)
                return prop.GetValue(obj);

            return null;

        }

        public static void SetPropertyValue(this object obj, string name, object value)
        {
            var prop = obj.GetType().GetProperty(name);
            if (prop != null)
            {
                prop.SetValue(obj, value);
            }
        }
    }
}