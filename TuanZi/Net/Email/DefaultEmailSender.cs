using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Threading.Tasks;
using TuanZi.Collections;
using TuanZi.Dependency;
using TuanZi.Core.Options;
using TuanZi.Exceptions;
using TuanZi.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace TuanZi.Net.Email
{
    public class DefaultEmailSender : IEmailSender, ISingletonDependency
    {
        private readonly IServiceProvider _provider;
        public DefaultEmailSender(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected virtual MailMessage BuildMailMessage(EmailMessage original)
        {
            MailMessage msg = new MailMessage();

			if (String.IsNullOrEmpty(original.Subject))
			{
				throw new MailSenderException("Required subject is missing!");
			}
				
			msg.Subject = original.Subject;
			msg.IsBodyHtml = original.BodyFormat == MailBodyFormat.Html;

            if (original.AltText.HasValue())
            {
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(original.AltText, new ContentType("text/html")));
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(original.Body, new ContentType("text/plain")));
            }
            else
            {
                msg.Body = original.Body;
            }

            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.None;

            msg.From = original.From.ToMailAddress();

			msg.To.AddRange(original.To.Where(x => x.Address.HasValue()).Select(x => x.ToMailAddress()));
			msg.CC.AddRange(original.Cc.Where(x => x.Address.HasValue()).Select(x => x.ToMailAddress()));
			msg.Bcc.AddRange(original.Bcc.Where(x => x.Address.HasValue()).Select(x => x.ToMailAddress()));
			msg.ReplyToList.AddRange(original.ReplyTo.Where(x => x.Address.HasValue()).Select(x => x.ToMailAddress()));

			msg.Attachments.AddRange(original.Attachments);

            if (original.Headers != null)
				msg.Headers.AddRange(original.Headers);
            

            msg.Priority = original.Priority;

            return msg;
        }

        #region IMailSender Members

        public void SendEmail(SmtpContext context, EmailMessage message)
        {
            context.CheckNotNull("context");
            message.CheckNotNull("message");
            
			
			using (var msg = this.BuildMailMessage(message))
			{
				using (var client = context.ToSmtpClient())
				{
					client.Send(msg);
				}
			}
        }

		public Task SendEmailAsync(SmtpContext context, EmailMessage message)
		{
            context.CheckNotNull("context");
            message.CheckNotNull("message");

            var client = context.ToSmtpClient();
			var msg = this.BuildMailMessage(message);

			return client.SendMailAsync(msg).ContinueWith(t => 
			{
				client.Dispose();
				msg.Dispose();
			});
		}


        public Task SendEmailAsync(string email, string subject, string body)
        {
            TuanOptions options = _provider.GetTuanOptions();
            MailSenderOptions mailSender = options.MailSender;
            if (mailSender == null)
            {
                throw new TuanException("Mail option does not exist, configure Tuan.MailSender node in appsetting.json");
            }

            string host = mailSender.Host,
             displayName = mailSender.DisplayName,
             userName = mailSender.UserName,
             password = mailSender.Password;
            SmtpClient client = new SmtpClient(host)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(userName, password)
            };

            string fromEmail = userName.Contains("@") ? userName : "{0}@{1}".FormatWith(userName, client.Host.Replace("smtp.", ""));
            MailMessage mail = new MailMessage
            {
                From = new MailAddress(fromEmail, displayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(email);
            return client.SendMailAsync(mail);
        }

        #endregion

    }
}
