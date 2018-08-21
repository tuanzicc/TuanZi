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
        private static readonly Type[] s_predefinedTypes = new Type[] { typeof(string), typeof(decimal), typeof(DateTime), typeof(TimeSpan), typeof(Guid) };
        private static readonly Type[] s_predefinedGenericTypes = new Type[] { typeof(Nullable<>) };

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

        public static string AssemblyQualifiedNameWithoutVersion(this Type type)
        {
            type.CheckNotNull(nameof(type));
            if (type.AssemblyQualifiedName != null)
            {
                var strArray = type.AssemblyQualifiedName.Split(new char[] { ',' });
                return string.Format("{0}, {1}", strArray[0], strArray[1]);
            }

            return null;
        }

        public static bool IsSequenceType(this Type seqType)
        {
            return (
                (((seqType != typeof(string))
                && (seqType != typeof(byte[])))
                && (seqType != typeof(char[])))
                && (FindIEnumerable(seqType) != null));
        }

        public static bool IsPredefinedSimpleType(this Type type)
        {
            if ((type.IsPrimitive && (type != typeof(IntPtr))) && (type != typeof(UIntPtr)))
            {
                return true;
            }
            if (type.IsEnum)
            {
                return true;
            }

            return s_predefinedTypes.Any(t => t == type);
        }

        public static bool IsStruct(this Type type)
        {
            if (type.IsValueType)
            {
                return !type.IsPredefinedSimpleType();
            }
            return false;
        }

        public static bool IsPredefinedGenericType(this Type type)
        {
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }
            else
            {
                return false;
            }

            return s_predefinedGenericTypes.Any(t => t == type);
        }

        public static bool IsPredefinedType(this Type type)
        {
            if ((!IsPredefinedSimpleType(type) && !IsPredefinedGenericType(type)) && ((type != typeof(byte[]))))
            {
                return (string.Compare(type.FullName, "System.Xml.Linq.XElement", StringComparison.Ordinal) == 0);
            }
            return true;
        }

        public static bool IsInteger(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsNullable(this Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsConstructable(this Type type)
        {
            type.CheckNotNull(nameof(type));

            if (type.IsAbstract || type.IsInterface || type.IsArray || type.IsGenericTypeDefinition || type == typeof(void))
                return false;

            if (!HasDefaultConstructor(type))
                return false;

            return true;
        }
        
        public static bool IsAnonymous(this Type type)
        {
            if (type.IsGenericType)
            {
                var d = type.GetGenericTypeDefinition();
                if (d.IsClass && d.IsSealed && d.Attributes.HasFlag(TypeAttributes.NotPublic))
                {
                    var attributes = d.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false);
                    if (attributes != null && attributes.Length > 0)
                    {
                        //WOW! We have an anonymous type!!!
                        return true;
                    }
                }
            }
            return false;
        }
        
        public static bool HasDefaultConstructor(this Type type)
        {
            type.CheckNotNull(nameof(type));

            if (type.IsValueType)
                return true;

            return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Any(ctor => ctor.GetParameters().Length == 0);
        }

        public static bool IsSubClass(this Type type, Type check)
        {
            Type implementingType;
            return IsSubClass(type, check, out implementingType);
        }

        public static bool IsSubClass(this Type type, Type check, out Type implementingType)
        {
            type.CheckNotNull(nameof(type));

            check.CheckNotNull(nameof(check));

            return IsSubClassInternal(type, type, check, out implementingType);
        }

        private static bool IsSubClassInternal(Type initialType, Type currentType, Type check, out Type implementingType)
        {
            if (currentType == check)
            {
                implementingType = currentType;
                return true;
            }

            // don't get interfaces for an interface unless the initial type is an interface
            if (check.IsInterface && (initialType.IsInterface || currentType == initialType))
            {
                foreach (Type t in currentType.GetInterfaces())
                {
                    if (IsSubClassInternal(initialType, t, check, out implementingType))
                    {
                        // don't return the interface itself, return it's implementor
                        if (check == implementingType)
                            implementingType = currentType;

                        return true;
                    }
                }
            }

            if (currentType.IsGenericType && !currentType.IsGenericTypeDefinition)
            {
                if (IsSubClassInternal(initialType, currentType.GetGenericTypeDefinition(), check, out implementingType))
                {
                    implementingType = currentType;
                    return true;
                }
            }

            if (currentType.BaseType == null)
            {
                implementingType = null;
                return false;
            }

            return IsSubClassInternal(initialType, currentType.BaseType, check, out implementingType);
        }

        /// <summary>
        /// Gets the underlying type of a <see cref="Nullable{T}" /> type.
        /// </summary>
        public static Type GetNonNullableType(this Type type)
        {
            if (!IsNullable(type))
            {
                return type;
            }
            return type.GetGenericArguments()[0];
        }


        /// <summary>
        /// Given a MethodBase for a property's get or set method,
        /// return the corresponding property info.
        /// </summary>
        /// <param name="method">MethodBase for the property's get or set method.</param>
        /// <returns>PropertyInfo for the property, or null if method is not part of a property.</returns>
        public static PropertyInfo GetPropertyFromMethod(this MethodBase method)
        {
            method.CheckNotNull(nameof(method));

            PropertyInfo property = null;
            if (method.IsSpecialName)
            {
                Type containingType = method.DeclaringType;
                if (containingType != null)
                {
                    if (method.Name.StartsWith("get_", StringComparison.InvariantCulture) ||
                        method.Name.StartsWith("set_", StringComparison.InvariantCulture))
                    {
                        string propertyName = method.Name.Substring(4);
                        property = containingType.GetProperty(propertyName);
                    }
                }
            }
            return property;
        }

        internal static Type FindIEnumerable(this Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                var args = seqType.GetGenericArguments();
                foreach (var arg in args)
                {
                    var ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                        return ienum;
                }
            }
            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null)
                        return ienum;
                }
            }
            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
                return FindIEnumerable(seqType.BaseType);
            return null;
        }

        public static T PropertyValue<T>(this Type obj, string propName)
        {
            var property = obj.GetProperty(propName);
            if (property != null)
                return (T)property.GetValue(propName, null);
            return default(T);
        }

        public static Type GetElementType(this Type type)
        {
            if (!type.IsPredefinedSimpleType())
            {
                if (type.HasElementType)
                {
                    return GetElementType(type.GetElementType());
                }
                if (type.IsPredefinedGenericType())
                {
                    return GetElementType(type.GetGenericArguments()[0]);
                }
                Type type2 = type.FindIEnumerable();
                if (type2 != null)
                {
                    Type type3 = type2.GetGenericArguments()[0];
                    return GetElementType(type3);
                }
            }

            return type;
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