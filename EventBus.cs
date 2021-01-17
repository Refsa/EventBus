using System;
using System.Collections.Generic;

namespace Refsa.EventBus
{
    public class EventBus
    {
        Dictionary<System.Type, IHandler<IMessage>> messageHandlers;

        Stack<BusLock> incomingMessages;
        Stack<System.Action> toResolve;
        bool lockDispatch => incomingMessages.Count > 0;

        public Stack<BusLock> IncomingMessages => incomingMessages;

        public EventBus()
        {
            messageHandlers = new Dictionary<System.Type, IHandler<IMessage>>();
            incomingMessages = new Stack<BusLock>();
            toResolve = new Stack<System.Action>();

            Sub<LockBusMessage>(OnLockBus);
            Sub<UnlockBusMessage>(OnUnlockBus);
        }

        void OnLockBus(LockBusMessage message)
        {
            incomingMessages.Push(BusLock.Lock());
        }

        void OnUnlockBus(UnlockBusMessage message)
        {
            while (incomingMessages.Count > 0)
            {
                var state = incomingMessages.Pop();

                if (state.Locked)
                {
                    break;
                }

                if (state.Callback != null)
                {
                    toResolve.Push(state.Callback);
                }
            }

            while (toResolve.Count > 0)
            {
                toResolve.Pop().Invoke();
            }
        }

        public bool IsLocked()
        {
            return lockDispatch;
        }

        public void Pub<TMessage>(TMessage message) where TMessage : IMessage
        {
            if (lockDispatch && !(message is IOverrideBusLock))
            {
                incomingMessages.Push(BusLock.Message(() => Pub(message), message));
                return;
            }

            if (messageHandlers.TryGetValue(typeof(TMessage), out var handler))
            {
                handler.Pub(message);
            }
        }

        public void Pub(object message)
        {
            var imessage = message as IMessage;
            if (imessage == null) return;

            if (lockDispatch && !(message is IOverrideBusLock))
            {
                incomingMessages.Push(BusLock.Message(() => Pub(imessage), imessage));
                return;
            }

            if (messageHandlers.TryGetValue(message.GetType(), out var handler))
            {
                handler.Pub(imessage);
            }
        }

        public void Pub<TMessage, HTarget>(TMessage message) where TMessage : IMessage
        {
            if (lockDispatch && !(message is IOverrideBusLock))
            {
                incomingMessages.Push(BusLock.Message(() => Pub<TMessage, HTarget>(message), message));
                return;
            }

            if (messageHandlers.TryGetValue(typeof(TMessage), out var handler))
            {
                handler.Pub<HTarget>(message);
            }
        }

        public void Pub<TMessage>(TMessage message, object target) where TMessage : IMessage
        {
            if (lockDispatch && !(message is IOverrideBusLock))
            {
                incomingMessages.Push(BusLock.Message(() => Pub(message, target), message));
                return;
            }

            if (messageHandlers.TryGetValue(typeof(TMessage), out var handler))
            {
                handler.Pub(message, target);
            }
        }

        public void Sub<TMessage>(System.Action<TMessage> callback) where TMessage : IMessage
        {
            if (!messageHandlers.TryGetValue(typeof(TMessage), out var handler))
            {
                messageHandlers.Add(typeof(TMessage), new MessageHandler<TMessage, IMessage>());
            }

            messageHandlers[typeof(TMessage)].Sub(callback);
        }
        
        public void UnSub<TMessage>(System.Action<TMessage> callback) where TMessage : IMessage
        {
            if (messageHandlers.TryGetValue(typeof(TMessage), out var handler))
            {
                handler.UnSub(callback);
            }
        }
    }
}