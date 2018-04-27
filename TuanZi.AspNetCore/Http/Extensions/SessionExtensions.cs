using Microsoft.AspNetCore.Http;
using TuanZi.Json;
public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, value.ToJsonString());
    }

    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default(T) : value.ToJsonObject<T>();
    }
}