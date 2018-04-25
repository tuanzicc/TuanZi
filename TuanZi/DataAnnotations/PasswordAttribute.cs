using System;
using System.ComponentModel.DataAnnotations;


namespace TuanZi.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PasswordAttribute : DataTypeAttribute
    {
        private string _value;

        public PasswordAttribute()
            : base(DataType.Password)
        {
            RequiredLength = 6;
            RequiredDigit = true;
            CanOnlyDigit = false;
            RequiredLowercase = true;
            RequiredUppercase = false;
        }

        public int RequiredLength { get; set; }

        public bool RequiredDigit { get; set; }

        public bool CanOnlyDigit { get; set; }

        public bool RequiredLowercase { get; set; }

        public bool RequiredUppercase { get; set; }

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
            return !RequiredUppercase || input.IsMatch(@"[A-Z]");
        }

        public override string FormatErrorMessage(string name)
        {
            name.CheckNotNullOrEmpty("name" );
            if (_value.Length < RequiredLength)
            {
                return "{0} length must be greater than {1} bits".FormatWith(name, RequiredLength);
            }
            if (RequiredDigit && !_value.IsMatch(@"[0-9]"))
            {
                return "{0} must contain digits".FormatWith(name);
            }
            if (!CanOnlyDigit && _value.IsMatch(@"^[0-9]+$"))
            {
                return "{0} not allowed to be all digits";
            }
            if (RequiredLowercase && !_value.IsMatch(@"[a-z]"))
            {
                return "{0} must contain lowercase letters".FormatWith(name);
            }
            if (RequiredUppercase && !_value.IsMatch(@"[A-Z]"))
            {
                return "{0} must contain uppercase letters".FormatWith(name);
            }
            return base.FormatErrorMessage(name);
        }

        #endregion
    }
}