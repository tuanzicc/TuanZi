using System.Threading.Tasks;


namespace TuanZi.AspNetCore.SignalR
{
    public interface IConnectionUserCache
    {
        Task<ConnectionUser> GetUser(string userName);

        Task<string[]> GetConnectionIds(string userName);

        Task SetUser(string userName, ConnectionUser user);

        Task AddConnectionId(string userName, string connectionId);

        Task RemoveConnectionId(string userName, string connectionId);

        Task RemoveUser(string userName);
    }
}