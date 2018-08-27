using TuanZi.EventBuses;


namespace TuanZi.Identity.Events
{
    public class OnlineUserCacheRemoveEventData : EventDataBase
    {
        public string[] UserNames { get; set; } = new string[0];
    }
}