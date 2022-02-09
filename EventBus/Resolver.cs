using System.Collections.Generic;

namespace Refsa.EventBus
{
    public interface IResolver
    {
        MessageHandler<TMessage> GetResolver<TMessage>() where TMessage : IMessage;
    }

    public class DictionaryResolver : IResolver
    {
        Dictionary<System.Type, IHandler<IMessage>> resolvers;

        public DictionaryResolver()
        {
            resolvers = new Dictionary<System.Type, IHandler<IMessage>>();
        }

        public MessageHandler<TMessage> GetResolver<TMessage>() where TMessage : IMessage
        {
            if (!resolvers.TryGetValue(typeof(TMessage), out var resolver))
            {
                resolver = new MessageHandler<TMessage>();
                resolvers.Add(typeof(TMessage), resolver);
            }

            return (MessageHandler<TMessage>)resolver;
        }
    }

    public class SparseSetResolver : IResolver
    {
        class SparseSet
        {
            int max;
            int count;
            int[] dense;
            int[] sparse;

            public SparseSet(int max)
            {
                this.max = max;
                this.count = 0;
                this.sparse = new int[max];
                this.dense = new int[max];
            }

            public int Add(int value)
            {
                if (value >= max)
                {
                    Grow();
                }

                if (value >= 0 && !Contains(value))
                {
                    dense[count] = value;
                    sparse[value] = count;
                    count++;
                    return count - 1;
                }

                return Get(value);
            }

            public int Get(int value)
            {
                if (value >= 0 && value < max && Contains(value))
                {
                    return sparse[value];
                }

                return -1;
            }

            public void Remove(int value)
            {
                if (Contains(value))
                {
                    dense[sparse[value]] = dense[count - 1];
                    sparse[dense[count - 1]] = sparse[value];
                    count--;
                }
            }

            public bool Contains(int value)
            {
                if (value >= max || value < 0) return false;

                return sparse[value] < count && dense[sparse[value]] == value;
            }

            public void Clear()
            {
                count = 0;
            }

            void Grow()
            {
                max = max * 2 + 1;
                var new_d = new int[max];
                var new_s = new int[max];
                dense.CopyTo(new_d, 0);
                sparse.CopyTo(new_s, 0);

                dense = new_d;
                sparse = new_s;
            }
        }

        static volatile int messageTypeCounter;
        static class MessageType<TMessage> where TMessage : IMessage
        {
            static readonly int id = messageTypeCounter++;
            public static int ID => id;
        }

        SparseSet indices;
        List<IHandler<IMessage>> handlers;

        public SparseSetResolver()
        {
            indices = new SparseSet(10);
            handlers = new List<IHandler<IMessage>>();
        }

        public MessageHandler<TMessage> GetResolver<TMessage>() where TMessage : IMessage
        {
            int mid = MessageType<TMessage>.ID;
            int idx = indices.Add(mid);

            if (idx >= handlers.Count)
            {
                handlers.Add(new MessageHandler<TMessage>());
            }
            return (MessageHandler<TMessage>)handlers[idx];
        }
    }
}