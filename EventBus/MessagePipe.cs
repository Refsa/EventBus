using System.Collections.Generic;

namespace Refsa.EventBus
{
    public class MessagePipe
    {
        MessageBus bus;

        MessagePipe parent;
        List<MessagePipe> children;

        public MessagePipe(MessagePipe parent = null)
        {
            this.parent = parent;
            parent?.AddChild(this);
            children = new List<MessagePipe>();
            bus = new MessageBus();
        }

        public void AddChild(MessagePipe messagePipe)
        {
            children.Add(messagePipe);
        }

        public void Sub<TMessage>(MessageHandlerDelegates.MessageHandler<TMessage> callback) where TMessage : IMessage
        {
            bus.Sub(callback);
        }

        public void PubDown<TMessage>(in TMessage message) where TMessage : IMessage
        {
            bus.Pub(message);
            foreach (var child in children)
            {
                child.PubDown(message);
            }
        }

        public void PubUp<TMessage>(in TMessage message) where TMessage : IMessage
        {
            bus.Pub(message);
            parent?.PubUp(message);
        }

        public void PubSide<TMessage>(in TMessage message) where TMessage : IMessage
        {
            parent?.PubDown(message, this);
        }

        void PubDown<TMessage>(in TMessage message, MessagePipe exclude) where TMessage : IMessage
        {
            foreach (var child in children)
            {
                if (child == exclude) continue;
                child.PubDown(message);
            }
        }
    }
}