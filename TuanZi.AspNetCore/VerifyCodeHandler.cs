using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Extensions;


namespace TuanZi.AspNetCore
{
    public static class VerifyCodeHandler
    {
        private const string Separator = "#$#";

        public static bool CheckCode(string code, string id, bool removeIfSuccess = true)
        {
            if (string.IsNullOrEmpty(code))
            {
                return false;
            }

            string key = $"{TuanConstants.VerifyCodeKeyPrefix}_{id}";
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            bool flag = code.Equals(cache.GetString(key), StringComparison.OrdinalIgnoreCase);
            if (removeIfSuccess && flag)
            {
                cache.Remove(key);
            }
            return flag;
        }

        public static void SetCode(string code, out string id)
        {
            id = Guid.NewGuid().ToString("N");
            string key = $"{TuanConstants.VerifyCodeKeyPrefix}_{id}";
            IDistributedCache cache = ServiceLocator.Instance.GetService<IDistributedCache>();
            const int seconds = 60 * 3;
            cache.SetString(key, code, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(seconds) });
        }

        public static string GetImageString(Image image, string id)
        {
            Check.NotNull(image, nameof(image));
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                byte[] bytes = ms.ToArray();
                string str = $"data:image/png;base64,{bytes.ToBase64String()}{Separator}{id}";
                return str.ToBase64String();
            }
        }
    }
}