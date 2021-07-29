using System.Collections.Generic;

namespace Refsa.EventBus
{
    public class EventBus
    {
        public EventBus()
        {

        }

        public void Pub<TMessage>(in TMessage message) where TMessage : IMessage
        {
            HandlerResolver<TMessage>.Get().Pub(message);
        }

        public void Pub<TMessage, HTarget>(in TMessage message) where TMessage : IMessage
        {
            HandlerResolver<TMessage>.Get().Pub<HTarget>(message);
        }

        public void Pub<TMessage>(in TMessage message, object target) where TMessage : IMessage
        {
            HandlerResolver<TMessage>.Get().Pub(message, target);
        }

        public void Sub<TMessage>(MessageHandlerDelegates.MessageHandler<TMessage> callback) where TMessage : IMessage
        {
            HandlerResolver<TMessage>.Get().Sub(callback);
        }
        
        public void UnSub<TMessage>(MessageHandlerDelegates.MessageHandler<TMessage> callback) where TMessage : IMessage
        {
            HandlerResolver<TMessage>.Get().UnSub(callback);
        }
    }
}