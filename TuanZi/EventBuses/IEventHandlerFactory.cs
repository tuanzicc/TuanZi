using TuanZi.EventBuses.Internal;

namespace TuanZi.EventBuses
{
    public interface IEventHandlerFactory
    {
        EventHandlerDisposeWrapper GetHandler();
    }
}