using System;
using System.Threading.Tasks;

namespace TuanZi.Net.Email
{    
    public interface IEmailSender
    {
        void SendEmail(SmtpContext smtpContext, EmailMessage message);
		Task SendEmailAsync(SmtpContext smtpContext, EmailMessage message); 
    }
}
