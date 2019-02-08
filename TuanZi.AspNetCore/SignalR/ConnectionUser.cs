using System.Collections.Generic;


namespace TuanZi.AspNetCore.SignalR
{
    public class ConnectionUser
    {
        public string UserName { get; set; }

        public ICollection<string> ConnectionIds { get; set; } = new List<string>();
    }
}