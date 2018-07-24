namespace TuanZi.Identity
{
    public class OnlineUser
    {
        public OnlineUser()
        {
            Roles = new string[0];
        }

        public string Id { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Email { get; set; }

        public string HeadImg { get; set; }

        public bool IsAdmin { get; set; }

        public string[] Roles { get; set; }
    }
}