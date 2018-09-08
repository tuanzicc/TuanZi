using System;
using System.Threading.Tasks;


namespace TuanZi.Identity
{
    public interface IOnlineUserProvider
    {
        Task<OnlineUser> Create(IServiceProvider provider, string userName);
    }
}