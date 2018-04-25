
using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;


namespace TuanZi.Filter
{
    public class SortCondition
    {
        public SortCondition(string sortField)
            : this(sortField, ListSortDirection.Ascending)
        { }

        public SortCondition(string sortField, ListSortDirection listSortDirection)
        {
            SortField = sortField;
            ListSortDirection = listSortDirection;
        }

        public string SortField { get; set; }

        public ListSortDirection ListSortDirection { get; set; }
    }


    public class SortCondition<T> : SortCondition
    {
        public SortCondition(Expression<Func<T, object>> keySelector)
            : this(keySelector, ListSortDirection.Ascending)
        { }

        public SortCondition(Expression<Func<T, object>> keySelector, ListSortDirection listSortDirection)
            : base(GetPropertyName(keySelector), listSortDirection)
        { }

        private static string GetPropertyName(Expression<Func<T, object>> keySelector)
        {
            string param = keySelector.Parameters.First().Name;
            string operand = (((dynamic)keySelector.Body).Operand).ToString();
            operand = operand.Substring(param.Length + 1, operand.Length - param.Length - 1);
            return operand;
        }
    }
}