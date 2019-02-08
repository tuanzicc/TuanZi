using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

using TuanZi.Data;
using TuanZi.Dependency;
using TuanZi.Extensions;


namespace TuanZi.AspNetCore
{
    [Dependency(ServiceLifetime.Singleton, TryAdd = true)]
    public class VerifyCodeService : IVerifyCodeService
    {
        private const string Separator = "#$#";
        private readonly IDistributedCache _cache;

        public VerifyCodeService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public bool CheckCode(string code, string id, bool removeIfSuccess = true)
        {
            if (string.IsNullOrEmpty(code))
            {
                return false;
            }

            string key = $"{TuanConstants.VerifyCodeKeyPrefix}_{id}";
            bool flag = code.Equals(_cache.GetString(key), StringComparison.OrdinalIgnoreCase);
            if (removeIfSuccess && flag)
            {
                _cache.Remove(key);
            }

            return flag;
        }

        public void SetCode(string code, out string id)
        {
            id = Guid.NewGuid().ToString("N");
            string key = $"{TuanConstants.VerifyCodeKeyPrefix}_{id}";
            const int seconds = 60 * 3;
            _cache.SetString(key, code, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(seconds) });
        }

        public string GetImageString(Image image, string id)
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