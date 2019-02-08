

using TuanZi.Core.Packs;
using TuanZi.EventBuses;


namespace TuanZi.Audits
{
    [DependsOnPacks(typeof(EventBusPack))]
    public abstract class AuditPackBase : TuanPack
    {
        public override PackLevel Level => PackLevel.Application;
    }
}