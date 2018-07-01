using System;
using System.Net;
using System.Net.Mail;
using TuanZi.Data;

namespace TuanZi.Net.Email
{
    public class SmtpContext
    {

        public SmtpContext(string host, int port = 25)
        {
            Check.NotNullOrEmpty(host, "host");
            Check.GreaterThan(port, "port", 0);
			
			this.Host = host;
            this.Port = port;
        }

        public SmtpContext(string host, int port, bool enableSsl, string password, bool useDefaultCredentials, string username)
        {
            Check.NotNullOrEmpty(host, "host");
            Check.GreaterThan(port, "port", 0);

            this.Host = host;
            this.Port = port;
            this.EnableSsl = enableSsl;
            this.Password = password;
            this.UseDefaultCredentials = useDefaultCredentials;
            this.Username = username;
        }

        public bool UseDefaultCredentials 
		{ 
			get; 
			set; 
		}

		public string Host 
		{ 
			get; 
			set; 
		}

		public int Port 
		{ 
			get; 
			set; 
		}

		public string Username 
		{ 
			get; 
			set; 
		}

		public string Password 
		{ 
			get; 
			set; 
		}

		public bool EnableSsl 
		{ 
			get; 
			set; 
		}

		public SmtpClient ToSmtpClient()
		{
			var smtpClient = new SmtpClient(this.Host, this.Port);

			smtpClient.UseDefaultCredentials = this.UseDefaultCredentials;
			smtpClient.EnableSsl = this.EnableSsl;
			if (this.UseDefaultCredentials)
			{
				smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
			}
			else
			{
				smtpClient.Credentials = new NetworkCredential(this.Username, this.Password);
			}

			return smtpClient;
		}

    }
}
