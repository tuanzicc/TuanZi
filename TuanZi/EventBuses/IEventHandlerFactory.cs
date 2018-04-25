namespace TuanZi.EventBuses
{
    public interface IEventHandlerFactory
    {
        IEventHandler GetHandler();

        void ReleaseHandler(IEventHandler handler);
    }
}