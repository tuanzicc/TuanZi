using TuanZi.Collections;


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