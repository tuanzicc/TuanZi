using System.Threading;
using System.Threading.Tasks;

using TuanZi.Dependency;


namespace TuanZi.EventBuses
{
    [IgnoreDependency]
    public interface IEventHandler
    {
        bool CanHandle(IEventData eventData);

        void Handle(IEventData eventData);

        Task HandleAsync(IEventData eventData, CancellationToken cancelToken = default(CancellationToken));
    }


    [IgnoreDependency]
    public interface IEventHandler<in TEventData> : IEventHandler where TEventData : IEventData
    {
        void Handle(TEventData eventData);

        Task HandleAsync(TEventData eventData, CancellationToken cancelToken = default(CancellationToken));
    }
}