using TuanZi.Collections;
using TuanZi.Extensions;

namespace TuanZi.Caching
{
    public class StringCacheKeyGenerator : ICacheKeyGenerator
    {
        public string GetKey(params object[] args)
        {
            args.CheckNotNullOrEmpty("args");
            return args.ExpandAndToString("-");
        }
    }
}