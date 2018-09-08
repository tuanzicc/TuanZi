using System.Collections.Generic;

namespace TuanZi.Identity
{
    public class OnlineUser
    {
        public OnlineUser()
        {
            Roles = new string[0];
            ExtendData = new Dictionary<string, object>();
        }

        public string Id { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Email { get; set; }

        public string HeadImg { get; set; }

        public bool IsAdmin { get; set; }

        public string[] Roles { get; set; }

        public IDictionary<string, object> ExtendData { get; }
    }
}