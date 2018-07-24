using TuanZi.Core.Packs;


namespace TuanZi.Core.Builders
{
    public static class TuanBuilderExtensions
    {
        public static ITuanBuilder AddCorePack(this ITuanBuilder builder)
        {
            return builder.AddPack<TuanPack>();
        }
    }
}