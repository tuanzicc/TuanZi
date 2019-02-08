using System.Security.Claims;

using Microsoft.AspNetCore.SignalR;


namespace TuanZi.AspNetCore.SignalR
{
    public class UserNameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}