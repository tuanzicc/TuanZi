using System;


namespace TuanZi.EventBuses
{
    public abstract class EventDataBase : IEventData
    {
        protected EventDataBase()
        {
            Id = Guid.NewGuid();
            EventTime = DateTime.Now;
        }

        public Guid Id { get; }

        public DateTime EventTime { get; }

        public object EventSource { get; set; }
    }
}