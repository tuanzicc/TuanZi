
namespace TuanZi.Filter
{
    public class FilterRule
    {
        #region Constructor

        public FilterRule()
            : this(null, null)
        { }

        public FilterRule(string field, object value)
            : this(field, value, FilterOperate.Equal)
        { }

        public FilterRule(string field, object value, string operateCode)
            : this(field, value, FilterHelper.GetFilterOperate(operateCode))
        { }

        public FilterRule(string field, object value, FilterOperate operate)
        {
            Field = field;
            Value = value;
            Operate = operate;
        }

        #endregion

        #region Properties

        public string Field { get; set; }

        public object Value { get; set; }

        public FilterOperate Operate { get; set; }

        #endregion


        #region Overrides of Object

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is FilterRule rule))
            {
                return false;
            }
            return rule.Field == Field && rule.Value == Value && rule.Operate == Operate;
        }

        #endregion
    }
}