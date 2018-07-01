
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TuanZi.Reflection;


namespace TuanZi.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum value)
        {
            Type type = value.GetType();
            MemberInfo member = type.GetMember(value.ToString()).FirstOrDefault();
            return member != null ? member.GetDescription() : value.ToString();
        }
    }
}