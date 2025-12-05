namespace SoulLike.ActorControllers
{
    public sealed class ActorEvent
    {
        private readonly MessageBroker messageBroker = new();

        public IMessageBroker Broker => messageBroker;
    }
}
