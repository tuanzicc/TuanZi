using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;

using TuanZi.Caching;
using TuanZi.Collections;


namespace TuanZi.AspNetCore.SignalR
{
    public class ConnectionUserCache : IConnectionUserCache
    {
        private readonly IDistributedCache _cache;

        public ConnectionUserCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        public virtual async Task<ConnectionUser> GetUser(string userName)
        {
            string key = GetKey(userName);
            return await _cache.GetAsync<ConnectionUser>(key);
        }

        public virtual async Task<string[]> GetConnectionIds(string userName)
        {
            ConnectionUser user = await GetUser(userName);
            return user?.ConnectionIds.ToArray() ?? new string[0];
        }

        public virtual async Task SetUser(string userName, ConnectionUser user)
        {
            string key = GetKey(userName);
            await _cache.SetAsync(key, user);
        }

        public virtual async Task AddConnectionId(string userName, string connectionId)
        {
            ConnectionUser user = await GetUser(userName) ?? new ConnectionUser() { UserName = userName };
            user.ConnectionIds.AddIfNotExist(connectionId);
            await SetUser(userName, user);
        }

        public virtual async Task RemoveConnectionId(string userName, string connectionId)
        {
            ConnectionUser user = await GetUser(userName);
            if (user == null || !user.ConnectionIds.Contains(connectionId))
            {
                return;
            }
            user.ConnectionIds.Remove(connectionId);
            if (user.ConnectionIds.Count == 0)
            {
                await RemoveUser(userName);
                return;
            }
            await SetUser(userName, user);
        }

        public virtual async Task RemoveUser(string userName)
        {
            string key = GetKey(userName);
            await _cache.RemoveAsync(key);
        }

        private static string GetKey(string userName)
        {
            return $"SignalR_Connection_User_{userName}";
        }
    }
}