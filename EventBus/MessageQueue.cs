
using System.Collections.Generic;

namespace Refsa.EventBus
{
    interface IMessageBag
    {
        void Clear();
    }

    public class MessageBag<TMessage> : IMessageBag
        where TMessage : IMessage
    {
        Queue<TMessage> queue;

        public int Count => queue.Count;

        public MessageBag(int capacity)
        {
            queue = new Queue<TMessage>(capacity);
        }

        public void Clear()
        {
            queue.Clear();
        }

        public bool Dequeue(out TMessage value)
        {
            if (queue.Count == 0)
            {
                value = default;
                return false;
            }

            value = queue.Dequeue();
            return true;
        }

        public void Enqueue(in TMessage value)
        {
            queue.Enqueue(value);
        }
    }

    public class MessageQueue
    {
        List<IMessageBag> bags;
        MessageBus bus;

        int queueCapacity;

        public MessageBus Bus => bus;

        public MessageQueue(int queueCapacity)
        {
            this.queueCapacity = queueCapacity;

            bags = new List<IMessageBag>();
            bus = new MessageBus();
        }

        public void Enqueue<TMessage>(in TMessage value)
            where TMessage : IMessage
        {
            int id = MessageBag.Factory<TMessage>.ID;

            while (bags.Count <= id)
            {
                bags.Add(null);
            }

            bags[id] ??= MessageBag.Factory<TMessage>.MakeBag(queueCapacity);
            (bags[id] as MessageBag<TMessage>).Enqueue(value);
        }

        public void DispatchSingle<TMessage>()
            where TMessage : IMessage
        {
            if (!HasBag<TMessage>(out int id)) return;

            if ((bags[id] as MessageBag<TMessage>).Dequeue(out var msg))
            {
                bus.Pub(msg);
            }
        }

        public void DispatchAll<TMessage>()
            where TMessage : IMessage
        {
            if (!HasBag<TMessage>(out int id)) return;

            var bagsT = bags[id] as MessageBag<TMessage>;
            while (bagsT.Dequeue(out var msg))
            {
                bus.Pub(msg);
            }
        }

        public void Clear<TMessage>()
            where TMessage : IMessage
        {
            if (!HasBag<TMessage>(out int id)) return;

            bags[id]?.Clear();
        }

        public void ClearAll()
        {
            foreach (var bag in bags)
            {
                bag?.Clear();
            }
        }

        bool HasBag<TMessage>(out int id)
            where TMessage : IMessage
        {
            id = MessageBag.Factory<TMessage>.ID;

            if (bags.Count < id) return false;
            if (bags[id] == null) return false;

            return true;
        }
    }

    static class MessageBag
    {
        static int counter;

        public static class Factory<TMessage>
            where TMessage : IMessage
        {
            static int id;

            static Factory()
            {
                id = counter++;
            }

            public static int ID => id;

            public static MessageBag<TMessage> MakeBag(int capacity)
            {
                return new MessageBag<TMessage>(capacity);
            }
        }
    }
}