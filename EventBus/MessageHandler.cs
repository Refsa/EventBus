using System;
using System.Linq;

namespace Refsa.EventBus
{
    public static class MessageHandlerDelegates
    {
        public delegate void MessageHandler<TMessage>(in TMessage message);
    }

    public interface IMessage { }
    interface IHandler<TMessage> { }

    class MessageHandler<TData> : IHandler<IMessage>
    {
        event MessageHandlerDelegates.MessageHandler<TData> observers;

        /// <summary>
        /// Pub a message to observers
        /// </summary>
        /// <param name="message"></param>
        public void Pub(in TData message)
        {
            if (observers == null) return;
            observers.Invoke(message);
        }

        /// <summary>
        /// Pub a message to specific type targets
        /// </summary>
        public void Pub<HTarget>(in TData message)
        {
            Pub(message, (in TData m) =>
            {
                foreach (MessageHandlerDelegates.MessageHandler<TData> observer in observers.GetInvocationList()
                    .Where(e => e.GetType() == typeof(HTarget)))
                {
                    observer.Invoke(m);
                }
            });
        }

        /// <summary>
        /// Pub a message to a specific target
        /// </summary>
        public void Pub(in TData message, object target)
        {
            Pub(message, (in TData m) =>
            {
                foreach (MessageHandlerDelegates.MessageHandler<TData> observer in observers.GetInvocationList()
                    .Where(e => e.Target == target))
                {
                    observer.Invoke(m);
                }
            });
        }

        /// <summary>
        /// Pub a message to the specified predicate
        /// </summary>
        void Pub(in TData message, MessageHandlerDelegates.MessageHandler<TData> action)
        {
            if (observers == null) return;
            action.Invoke(message);
        }

        /// <summary>
        /// Sub to messages
        /// </summary>
        /// <param name="callback"></param>
        public void Sub(MessageHandlerDelegates.MessageHandler<TData> callback)
        {
            observers += callback;
        }

        /// <summary>
        /// Unsub from messages
        /// </summary>
        /// <param name="callback"></param>
        public void UnSub(MessageHandlerDelegates.MessageHandler<TData> callback)
        {
            observers -= callback;
        }
    }
}