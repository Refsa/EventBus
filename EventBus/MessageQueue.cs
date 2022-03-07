
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

        public MessageBag()
        {
            queue = new Queue<TMessage>();
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
        Dictionary<int, IMessageBag> bags;
        MessageBus bus;

        public MessageBus Bus => bus;

        public MessageQueue(IResolver resolver = null)
        {
            bags = new Dictionary<int, IMessageBag>();
            if (resolver != null)
            {
                bus = new MessageBus(resolver);
            }
            else
            {
                bus = new MessageBus();
            }
        }

        public void Enqueue<TMessage>(in TMessage value)
            where TMessage : IMessage
        {
            int id = MessageBag.Factory<TMessage>.ID;
            if (!bags.TryGetValue(id, out var bag))
            {
                bag = MessageBag.Factory<TMessage>.MakeBag();
                bags.Add(id, bag);
            }

            (bag as MessageBag<TMessage>).Enqueue(value);
        }

        public void DispatchSingle<TMessage>()
            where TMessage : IMessage
        {
            int id = MessageBag.Factory<TMessage>.ID;
            if (!bags.TryGetValue(id, out var bag)) return;

            if ((bag as MessageBag<TMessage>).Dequeue(out var msg))
            {
                bus.Pub(msg);
            }
        }

        public void DispatchAll<TMessage>()
            where TMessage : IMessage
        {
            int id = MessageBag.Factory<TMessage>.ID;
            if (!bags.TryGetValue(id, out var bag)) return;

            var bagsT = bag as MessageBag<TMessage>;
            while (bagsT.Dequeue(out var msg))
            {
                bus.Pub(msg);
            }
        }

        public void Clear<TMessage>()
            where TMessage : IMessage
        {
            int id = MessageBag.Factory<TMessage>.ID;
            if (!bags.TryGetValue(id, out var bag)) return;
            bag.Clear();
        }

        public void ClearAll()
        {
            foreach (var bag in bags)
            {
                bag.Value.Clear();
            }
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

            public static MessageBag<TMessage> MakeBag()
            {
                return new MessageBag<TMessage>();
            }
        }
    }
}