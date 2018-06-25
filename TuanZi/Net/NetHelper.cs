using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TuanZi.Net
{
    public static class NetHelper
    {
        public static string GetLocalIPAddress()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                var endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }

        public static List<string> GetLocalIPAddresses()
        {
            var list = new List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    list.Add(ip.ToString());
                }
            }
            return list;
        }

        public static bool Ping(string hostNameOrAddress, bool error = false)
        {
            try
            {
                Ping ping = new Ping();
                PingOptions options = new PingOptions { DontFragment = true };
                string data = "Test Data";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 1000;
                PingReply reply = ping.Send(hostNameOrAddress, timeout, buffer, options);
                return reply != null && reply.Status == IPStatus.Success;
            }
            catch (PingException e)
            {
                if (error)
                    throw e;
                return false;
            }
        }

        public static async Task<bool> PingAsync(string hostNameOrAddress, bool error = false)
        {
            try
            {
                Ping ping = new Ping();
                PingOptions options = new PingOptions { DontFragment = true };
                string data = "Test Data";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 1000;
                PingReply reply = await ping.SendPingAsync(hostNameOrAddress, timeout, buffer, options);
                return reply != null && reply.Status == IPStatus.Success;
            }
            catch (PingException e)
            {
                if (error)
                    throw e;
                return false;
            }
        }

        public static async Task<PingReply> PingReplyAsync(string hostNameOrAddress, bool error = false)
        {
            try
            {
                Ping ping = new Ping();
                PingOptions options = new PingOptions { DontFragment = true };
                string data = "Test Data";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 1000;
                PingReply reply = await ping.SendPingAsync(hostNameOrAddress, timeout, buffer, options);
                return reply;
            }
            catch (PingException e)
            {
                if (error)
                    throw e;
                return null;
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
