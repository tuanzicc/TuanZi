using System;


namespace TuanZi.EventBuses
{
    public interface IEventData
    {
        Guid Id { get; }

        DateTime EventTime { get; }

        object EventSource { get; set; }
    }
}