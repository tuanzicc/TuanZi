using System.Threading.Tasks;


namespace TuanZi.Identity
{
    public interface IOnlineUserCache
    {
        OnlineUser GetOrRefresh(string userName);

        Task<OnlineUser> GetOrRefreshAsync(string userName);

        void Remove(params string[] userNames);
    }
}