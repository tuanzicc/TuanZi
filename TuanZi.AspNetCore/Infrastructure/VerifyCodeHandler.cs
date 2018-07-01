using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TuanZi.AspNetCore.Http;
using TuanZi.Dependency;

namespace TuanZi.AspNetCore.Infrastructure
{
    public static class VerifyCodeHandler
    {
        public static bool CheckCode(string code, bool cleanIfFited = false)
        {
            if (string.IsNullOrEmpty(code))
            {
                return false;
            }
            const string name = TuanConstants.VerifyCodeKeyPrefix;
            ISession session = ServiceLocator.Instance.HttpContext().Session;
            string sessionCode = session.GetString(name);
            bool fited = sessionCode != null && sessionCode.Equals(code, StringComparison.OrdinalIgnoreCase);
            if (fited && cleanIfFited)
            {
                session.Remove(name);
            }
            return fited;
        }

        public static void SetCode(string code)
        {
            const string name = TuanConstants.VerifyCodeKeyPrefix;
            ServiceLocator.Instance.HttpContext().Session.SetString(name, code);
        }
    }
}
