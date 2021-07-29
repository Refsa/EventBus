using System;
using System.Linq;

namespace Refsa.EventBus
{
    public static class MessageHandlerDelegates
    {
        public delegate void MessageHandler<TMessage>(in TMessage message);
    }

    class MessageHandler<MData, MType>
    {
        event MessageHandlerDelegates.MessageHandler<MData> observers;

        public void Pub(in MType message)
        {
            if (observers == null) return;

            Pub(message, (MessageHandlerDelegates.MessageHandler<MData>)observers.Invoke);
        }

        public void Pub<HTarget>(in MType message)
        {
            Pub(message, (in MData m) =>
            {
                foreach (MessageHandlerDelegates.MessageHandler<MData> observer in observers.GetInvocationList()
                    .Where(e => e.Target.GetType().FullName.Contains(typeof(HTarget).Name)))
                {
                    observer.Invoke(m);
                }
            });
        }

        public void Pub(in MType message, object target)
        {
            Pub(message, (in MData m) =>
            {
                foreach (MessageHandlerDelegates.MessageHandler<MData> observer in observers.GetInvocationList()
                    .Where(e => e.Target == target))
                {
                    observer.Invoke(m);
                }
            });
        }

        void Pub(in MType message, MessageHandlerDelegates.MessageHandler<MData> action)
        {
            if (observers == null)
            {
                return;
            }

            if (message is MData t)
            {
                action.Invoke(t);
                return;
            }
            throw new System.ArgumentException($"Message given to message handler is of wrong type\nShould be {typeof(MData)} but was {message.GetType()}");
        }

        public void Sub(MessageHandlerDelegates.MessageHandler<MData> callback)
        {
            observers += callback;
        }

        public void UnSub(MessageHandlerDelegates.MessageHandler<MData> callback)
        {
            observers -= callback;
        }
    }
}