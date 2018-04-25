using System;
using System.Linq;
using System.Reflection;


namespace TuanZi.Reflection
{
    public class PublicInstanceMethodInfoFinder : IMethodInfoFinder
    {
        public MethodInfo[] Find(Type type, Func<MethodInfo, bool> predicate)
        {
            return FindAll(type).Where(predicate).ToArray();
        }

        public MethodInfo[] FindAll(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }
    }
}