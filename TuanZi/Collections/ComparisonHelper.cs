using System;
using System.Collections.Generic;


namespace TuanZi.Collections
{
    public static class ComparisonHelper<T>
    {
        public static IComparer<T> CreateComparer<TV>(Func<T, TV> keySelector)
        {
            return new CommonComparer<TV>(keySelector);
        }

        public static IComparer<T> CreateComparer<TV>(Func<T, TV> keySelector, IComparer<TV> comparer)
        {
            return new CommonComparer<TV>(keySelector, comparer);
        }
        
        private class CommonComparer<TV> : IComparer<T>
        {
            private readonly IComparer<TV> _comparer;
            private readonly Func<T, TV> _keySelector;

            public CommonComparer(Func<T, TV> keySelector, IComparer<TV> comparer)
            {
                _keySelector = keySelector;
                _comparer = comparer;
            }

            public CommonComparer(Func<T, TV> keySelector)
                : this(keySelector, Comparer<TV>.Default)
            { }
            
            public int Compare(T x, T y)
            {
                return _comparer.Compare(_keySelector(x), _keySelector(y));
            }
            
        }
        
    }
}