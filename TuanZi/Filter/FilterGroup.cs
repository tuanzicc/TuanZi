
using System;
using System.Collections.Generic;
using System.Linq;
using TuanZi.Properties;


namespace TuanZi.Filter
{
    public class FilterGroup
    {
        private FilterOperate _operate;

        #region Constructor

        public FilterGroup()
            : this(FilterOperate.And)
        { }

        public FilterGroup(string operateCode)
            : this(FilterHelper.GetFilterOperate(operateCode))
        { }

        public FilterGroup(FilterOperate operate)
        {
            Operate = operate;
            Rules = new List<FilterRule>();
            Groups = new List<FilterGroup>();
        }

        #endregion

        public ICollection<FilterRule> Rules { get; set; }

        public ICollection<FilterGroup> Groups { get; set; }

        public FilterOperate Operate
        {
            get { return _operate; }
            set
            {
                if (value != FilterOperate.And && value != FilterOperate.Or)
                {
                    throw new InvalidOperationException(Resources.Filter_GroupOperateError);
                }
                _operate = value;
            }
        }

        public void AddRule(FilterRule rule)
        {
            if (Rules.All(m => !m.Equals(rule)))
            {
                Rules.Add(rule);
            }
        }
    }
}