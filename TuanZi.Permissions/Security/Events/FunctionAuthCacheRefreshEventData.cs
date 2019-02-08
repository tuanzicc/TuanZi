using System;

using TuanZi.EventBuses;


namespace TuanZi.Security.Events
{
    public class FunctionAuthCacheRefreshEventData : EventDataBase
    {
        public FunctionAuthCacheRefreshEventData()
        {
            FunctionIds = new Guid[0];
            UserNames = new string[0];
        }

        public Guid[] FunctionIds { get; set; }

        public string[] UserNames { get; set; }
    }
}