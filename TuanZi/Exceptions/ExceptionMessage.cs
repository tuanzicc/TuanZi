
using System;
using System.Text;


namespace TuanZi.Exceptions
{
    public class ExceptionMessage
    {
        #region Fields

        #endregion

        #region Constructor

        public ExceptionMessage(Exception e, string userMessage = null, bool isHideStackTrace = false)
        {
            UserMessage = string.IsNullOrEmpty(userMessage) ? e.Message : userMessage;

            StringBuilder sb = new StringBuilder();
            ExMessage = string.Empty;
            int count = 0;
            string appString = string.Empty;
            while (e != null)
            {
                if (count > 0)
                {
                    appString += "    ";
                }
                ExMessage = e.Message;
                sb.AppendLine(appString + "Message: " + e.Message);
                sb.AppendLine(appString + "Type: " + e.GetType().FullName);
                sb.AppendLine(appString + "Method: " + (e.TargetSite == null ? null : e.TargetSite.Name));
                sb.AppendLine(appString + "Source: " + e.Source);
                if (!isHideStackTrace && e.StackTrace != null)
                {
                    sb.AppendLine(appString + "StackTrace: " + e.StackTrace);
                }
                if (e.InnerException != null)
                {
                    sb.AppendLine(appString + "InnerException: ");
                    count++;
                }
                e = e.InnerException;
            }
            ErrorDetails = sb.ToString();
            sb.Clear();
        }

        #region Properties

        public string UserMessage { get; set; }

        public string ExMessage { get; private set; }

        public string ErrorDetails { get; private set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return ErrorDetails;
        }

        #endregion

        #endregion
    }
}