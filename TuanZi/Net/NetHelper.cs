using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;


namespace TuanZi.Net
{
    public static class NetHelper
    {
        public static bool Ping(string ip)
        {
            try
            {
                Ping ping = new Ping();
                PingOptions options = new PingOptions { DontFragment = true };
                string data = "Test Data";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 1000;
                PingReply reply = ping.Send(ip, timeout, buffer, options);
                return reply != null && reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
        }

        public static bool IsInternetConnected()
        {
            int i;
            bool state = InternetGetConnectedState(out i, 0);
            return state;
        }

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int connectionDescription, int reservedValue);
    }
}
