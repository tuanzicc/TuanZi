using System;
using System.ComponentModel.DataAnnotations;
using TuanZi.Extensions;

namespace TuanZi.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class IpAddressAttribute : ValidationAttribute
    {
        private bool _isMultiple = false;
        public IpAddressAttribute(bool isMultiple = false)
        {
            this._isMultiple = isMultiple;
            this.ErrorMessage = "Invalid IP Address";
        }

        public override bool IsValid(object value)
        {
            if (null == value) { return true; }
            string[] m = value.ToString().Split('.');

            if (4 != m.Length) { return false; }

            return (isnum(m[0], this._isMultiple) &&
                    isnum(m[1], this._isMultiple) &&
                    isnum(m[2], this._isMultiple) &&
                    isnum(m[3], this._isMultiple)
                   );
        }

        private bool isnum(string n, bool ismulti)
        {
            if (ismulti)
            {
                if ("*".Equals(n))
                {
                    return true;
                }
                else
                {
                    return isnum(n, false);
                }
            }
            else
            {
                int num = -1;
                if (int.TryParse(n, out num))
                {
                    return ((-1 < num) && (num < 256));
                }
                else
                {
                    return false;
                }
            }
        }
    }
}