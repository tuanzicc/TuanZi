using Microsoft.Extensions.Logging;


namespace TuanZi.EventBuses.Internal
{
    internal class PassThroughEventBus : EventBusBase
    {
        public PassThroughEventBus(IEventStore eventStore, ILogger<PassThroughEventBus> logger)
            : base(eventStore, logger)
        { }
    }
}