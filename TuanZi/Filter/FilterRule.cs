
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
    }
}