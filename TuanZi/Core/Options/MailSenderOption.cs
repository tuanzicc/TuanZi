using System;
using System.Collections.Generic;
using System.Text;

namespace TuanZi.Core.Options
{
    public class MailSenderOptions
    {
        public string Host { get; set; }
        
        public string SenderDisplayName { get; set; }
        
        public string SenderUserName { get; set; }
        
        public string SenderPassword { get; set; }
    }
}


