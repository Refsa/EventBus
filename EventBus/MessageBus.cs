using System.Collections.Generic;

namespace Refsa.EventBus
{
    public class MessageBus
    {
        object locker = new object();
        Dictionary<System.Type, IHandler<IMessage>> resolvers;

        public MessageBus()
        {
            resolvers = new Dictionary<System.Type, IHandler<IMessage>>();
        }

        /// <summary>
        /// Retreives or creates a new handler for message type TMessage
        /// </summary>
        MessageHandler<TMessage> GetResolver<TMessage>() where TMessage : IMessage
        {
            if (!resolvers.TryGetValue(typeof(TMessage), out var resolver))
            {
                resolver = new MessageHandler<TMessage>();

                lock (locker)
                {
                    resolvers.Add(typeof(TMessage), resolver);
                }
            }

            return (MessageHandler<TMessage>)resolver;
        }

        /// <summary>
        /// Pub a message
        /// </summary>
        /// <param name="message"></param>
        /// <typeparam name="TMessage"></typeparam>
        public void Pub<TMessage>(in TMessage message) where TMessage : IMessage
        {
            GetResolver<TMessage>().Pub(message);
        }

        /// <summary>
        /// Pub a message to targets of specific type
        /// </summary>
        public void Pub<TMessage, HTarget>(in TMessage message) where TMessage : IMessage
        {
            GetResolver<TMessage>().Pub<HTarget>(message);
        }

        /// <summary>
        /// Pub a message to a specific object
        /// </summary>
        public void Pub<TMessage>(in TMessage message, object target) where TMessage : IMessage
        {
            GetResolver<TMessage>().Pub(message, target);
        }

        /// <summary>
        /// Sub to message
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="TMessage"></typeparam>
        public void Sub<TMessage>(MessageHandlerDelegates.MessageHandler<TMessage> callback) where TMessage : IMessage
        {
            var resolver = GetResolver<TMessage>();
            lock (locker)
            {
                resolver.Sub(callback);
            }
        }

        /// <summary>
        /// Unsub from message
        /// </summary>
        /// <param name="callback"></param>
        /// <typeparam name="TMessage"></typeparam>
        public void UnSub<TMessage>(MessageHandlerDelegates.MessageHandler<TMessage> callback) where TMessage : IMessage
        {
            var resolver = GetResolver<TMessage>();
            lock (locker)
            {
                resolver.UnSub(callback);
            }
        }
    }
}