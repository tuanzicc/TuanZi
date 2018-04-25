namespace TuanZi.Caching
{
    public interface ICacheKeyGenerator
    {
        string GetKey(params object[] args);
    }
}