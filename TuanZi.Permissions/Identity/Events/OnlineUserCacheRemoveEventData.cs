using TuanZi.EventBuses;


namespace TuanZi.Identity.Events
{
    public class OnlineUserCacheRemoveEventData : EventDataBase
    {
        public string UserName { get; set; }
    }
}