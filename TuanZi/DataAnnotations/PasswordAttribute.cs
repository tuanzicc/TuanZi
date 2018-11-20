using System;
using System.ComponentModel.DataAnnotations;
using TuanZi.Extensions;

namespace TuanZi.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PasswordAttribute : DataTypeAttribute
    { 
        private string _value;

        public PasswordAttribute()
            : base(DataType.Password)
        {
            RequiredLength = 8;
            RequiredDigit = true;
            CanOnlyDigit = false;
            RequiredLowercase = false;
            RequiredUppercase = false;
            RequiredNonAlphanumeric = false;
        }

        public int RequiredLength { get; set; }

        public bool RequiredDigit { get; set; }

        public bool CanOnlyDigit { get; set; }

        public bool RequiredLowercase { get; set; }

        public bool RequiredUppercase { get; set; }
        public bool RequiredNonAlphanumeric { get; set; }

        #region Overrides of DataTypeAttribute

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            string input = value as string;
            if (input == null)
            {
                return false;
            }
            _value = input;
            if (input.Length < RequiredLength)
            {
                return false;
            }
            if (RequiredDigit && !input.IsMatch(@"[0-9]"))
            {
                return false;
            }
            if (!CanOnlyDigit && input.IsMatch(@"^[0-9]+$"))
            {
                return false;
            }
            if (RequiredLowercase && !input.IsMatch(@"[a-z]"))
            {
                return false;
            }
            if (RequiredUppercase && !input.IsMatch(@"[A-Z]"))
            {
                return false;
            }

            return !RequiredNonAlphanumeric || input.IsMatch(@"[^a-zA-Z\d\s:]");
        }

        //public override string FormatErrorMessage(string name)
        //{
        //    name.CheckNotNullOrEmpty("name" );
        //    if (_value.Length < RequiredLength)
        //    {
        //        return "{0} length must be greater than {1} charaters".FormatWith(name, RequiredLength);
        //    }
        //    if (RequiredDigit && !_value.IsMatch(@"[0-9]"))
        //    {
        //        return "{0} must have at least one numeric character".FormatWith(name);
        //    }
        //    if (!CanOnlyDigit && _value.IsMatch(@"^[0-9]+$"))
        //    {
        //        return "{0} not allowed to be all numeric characters";
        //    }
        //    if (RequiredLowercase && !_value.IsMatch(@"[a-z]"))
        //    {
        //        return "{0} must have at least one lowercase letter".FormatWith(name);
        //    }
        //    if (RequiredUppercase && !_value.IsMatch(@"[A-Z]"))
        //    {
        //        return "{0} must have at least one uppercase letter".FormatWith(name);
        //    }
        //    if (RequiredNonAlphanumeric && !_value.IsMatch(@"[^a-zA-Z\d\s:]"))
        //    {
        //        return "{0} must have at least one non alphanumeric character".FormatWith(name);
        //    }
        //    return base.FormatErrorMessage(name);
        //}

        #endregion
    }
}