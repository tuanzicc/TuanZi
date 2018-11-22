using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Net;

namespace TuanZi.Extensions
{
	public static class IPAddressExtensions
    { 
        public static bool IsIPv4MappedTo(this IPAddress ipAddress, string ipMask)
        {
            if (string.IsNullOrWhiteSpace(ipMask)) { return true; }
            string[] a = ipMask.ToString().Split('.');
            if (4 != a.Count()) { return true; }
            if (ipMask == "::1") { return true; }

            string[] b = ipAddress.ToString().Split('.');
            if (IPAddress.IsLoopback(ipAddress))
            {
                b = "127.0.0.1".Split('.');
            }
            int n = 0;
            if (a[0].Equals(b[0]) || "*".Equals(a[0])) { n++; }
            if (a[1].Equals(b[1]) || "*".Equals(a[1])) { n++; }
            if (a[2].Equals(b[2]) || "*".Equals(a[2])) { n++; }
            if (a[3].Equals(b[3]) || "*".Equals(a[3])) { n++; }

            return (4 == n);
        }
    }
    
}
