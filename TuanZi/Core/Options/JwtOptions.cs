using System;
using System.Collections.Generic;
using System.Text;

namespace TuanZi.Core.Options
{
    public class JwtOptions
    {
        public string Secret { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }
    }
}


