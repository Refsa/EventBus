
namespace Refsa.EventBus
{
    static class HandlerResolver<TMessage>
        where TMessage : IMessage
    {
        static MessageHandler<TMessage, IMessage> handler;

        static HandlerResolver()
        {
            handler = new MessageHandler<TMessage, IMessage>();
        }
 
        public static MessageHandler<TMessage, IMessage> Get()
        {
            return handler;
        }
    }
}