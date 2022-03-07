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

        public MessagePipe(IResolver resolver, MessagePipe parent = null)
        {
            this.parent = parent;
            parent?.AddChild(this);
            children = new List<MessagePipe>();
            bus = new MessageBus(resolver);
        }

        /// <summary>
        /// Sets the parent pipe
        /// </summary>
        public void SetParent(MessagePipe pipe)
        {
            this.parent = pipe;
        }

        /// <summary>
        /// Adds a child pipe
        /// </summary>
        public void AddChild(MessagePipe messagePipe)
        {
            children.Add(messagePipe);
        }

        /// <summary>
        /// Removes a child pipe
        /// </summary>
        public void RemoveChild(MessagePipe pipe)
        {
            children.Remove(pipe);
        }

        /// <summary>
        /// Sub to messages on this pipe
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="TMessage"></typeparam>
        public void Sub<TMessage>(MessageHandlerDelegates.MessageHandler<TMessage> callback) where TMessage : IMessage
        {
            bus.Sub(callback);
        }

        /// <summary>
        /// Unsub from a message on this pipe
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="TMessage"></typeparam>
        public void UnSub<TMessage>(MessageHandlerDelegates.MessageHandler<TMessage> callback) where TMessage : IMessage
        {
            bus.UnSub<TMessage>(callback);
        }

        /// <summary>
        /// Pub message to this message pipe
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="TMessage"></typeparam>
        public void Pub<TMessage>(in TMessage message) where TMessage : IMessage
        {
            bus.Pub(message);
        }

        /// <summary>
        /// Pub message to this pipe and the hierarchy below it
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="TMessage"></typeparam>
        public void PubDown<TMessage>(in TMessage message) where TMessage : IMessage
        {
            bus.Pub(message);
            foreach (var child in children)
            {
                child.PubDown(message);
            }
        }

        /// <summary>
        /// Pub message to this pipe and the hierarchy above it
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="TMessage"></typeparam>
        public void PubUp<TMessage>(in TMessage message) where TMessage : IMessage
        {
            bus.Pub(message);
            parent?.PubUp(message);
        }

        /// <summary>
        /// Pub message to this node hierarchy and its sibling hierarchies
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="TMessage"></typeparam>
        public void PubSide<TMessage>(in TMessage message) where TMessage : IMessage
        {
            parent?.PubDown(message, this);
        }

        /// <summary>
        /// Pub a message to the child hierarchy, ignoring the given pipe
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exclude"></param>
        /// <typeparam name="TMessage"></typeparam>
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