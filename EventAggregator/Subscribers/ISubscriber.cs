namespace EventAggregator
{
    public interface ISubscriber<TEventType>
    {
        void OnEventHandler(TEventType e);
    }
}