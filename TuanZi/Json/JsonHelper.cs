
using System;
using System.Text.RegularExpressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TuanZi.Json
{
    public static class JsonHelper
    {
        public static string JsonDateTimeFormat(string json)
        {
            json.CheckNotNullOrEmpty("json");
            json = Regex.Replace(json,
                @"\\/Date\((\d+)\)\\/",
                match =>
                {
                    DateTime dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                    dt = dt.ToLocalTime();
                    return dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
                });
            return json;
        }

        public static string ToJson(object @object, bool camelCase = false, bool indented = false)
        {
            @object.CheckNotNull("@object");

            JsonSerializerSettings settings = new JsonSerializerSettings();
            if (camelCase)
            {
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            if (indented)
            {
                settings.Formatting = Formatting.Indented;
            }
            string json = JsonConvert.SerializeObject(@object, settings);
            return JsonDateTimeFormat(json);
        }

        public static T FromJson<T>(string json)
        {
            json.CheckNotNullOrEmpty("json");

            json = JsonDateTimeFormat(json);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}