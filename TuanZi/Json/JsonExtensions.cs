using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TuanZi.Extensions;

namespace TuanZi.Json
{
    public static class JsonExtensions
    {
        public static string ToJsonString(this object obj, bool camelCase = false, bool indented = false)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            if (camelCase)
            {
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            if (indented)
            {
                settings.Formatting = Formatting.Indented;
            }
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T ToJsonObject<T>(this string str)
        {
            if (str.IsNullOrWhiteSpace())
                return default(T);
            return JsonConvert.DeserializeObject<T>(str);
        }

        public static object ToJsonObject(this string str)
        {
            if (str.IsNullOrWhiteSpace())
                return null;
            return JsonConvert.DeserializeObject(str);
        }
    }
}
