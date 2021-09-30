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

    class MessageHandler<TData, TType> : IHandler<IMessage>
    {
        event MessageHandlerDelegates.MessageHandler<TData> observers;

        public void Pub(in TType message)
        {
            if (observers == null) return;

            Pub(message, (MessageHandlerDelegates.MessageHandler<TData>)observers.Invoke);
        }

        public void Pub<HTarget>(in TType message)
        {
            Pub(message, (in TData m) =>
            {
                foreach (MessageHandlerDelegates.MessageHandler<TData> observer in observers.GetInvocationList()
                    .Where(e => e.Target.GetType().FullName.Contains(typeof(HTarget).Name)))
                {
                    observer.Invoke(m);
                }
            });
        }

        public void Pub(in TType message, object target)
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

        void Pub(in TType message, MessageHandlerDelegates.MessageHandler<TData> action)
        {
            if (observers == null)
            {
                return;
            }

            if (message is TData t)
            {
                action.Invoke(t);
                return;
            }
            throw new System.ArgumentException($"Message given to message handler is of wrong type\nShould be {typeof(TData)} but was {message.GetType()}");
        }

        public void Sub(MessageHandlerDelegates.MessageHandler<TData> callback)
        {
            observers += callback;
        }

        public void UnSub(MessageHandlerDelegates.MessageHandler<TData> callback)
        {
            observers -= callback;
        }
    }
}