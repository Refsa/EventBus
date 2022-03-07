using BenchmarkDotNet.Attributes;

namespace Refsa.EventBus.Bench
{
    [MemoryDiagnoser]
    public class MessageBusBench
    {
        struct SimpleMessage : IMessage
        {
            public int Value;
        }

        MessageBus messageBusDictResolver;
        MessageBus messageBusSparseSetResolver;
        MessageHandlerDelegates.MessageHandler<SimpleMessage> PubSimpleMessageDictResolverDelegate;
        MessageHandlerDelegates.MessageHandler<SimpleMessage> PubSimpleMessageSparseSetResolverDelegate;

        public MessageBusBench()
        {
            messageBusDictResolver = new MessageBus(new DictionaryResolver());
            messageBusSparseSetResolver = new MessageBus(new SparseSetResolver());
        }

        [Benchmark]
        public int PubSimpleMessageDictResolver_10M()
        {
            int value = 0;
            PubSimpleMessageDictResolverDelegate = (in SimpleMessage msg) =>
            {
                value = msg.Value;
            };
            messageBusDictResolver.Sub(PubSimpleMessageDictResolverDelegate);

            for (int i = 0; i < 10_000_000; i++)
            {
                messageBusDictResolver.Pub(new SimpleMessage { Value = 1234 });
            }

            messageBusDictResolver.UnSub(PubSimpleMessageDictResolverDelegate);
            return value;
        }

        [Benchmark]
        public int PubSimpleMessageSparseSetResolver_10M()
        {
            int value = 0;
            PubSimpleMessageSparseSetResolverDelegate = (in SimpleMessage msg) =>
            {
                value = msg.Value;
            };
            messageBusSparseSetResolver.Sub(PubSimpleMessageSparseSetResolverDelegate);

            for (int i = 0; i < 10_000_000; i++)
            {
                messageBusSparseSetResolver.Pub(new SimpleMessage { Value = 1234 });
            }

            messageBusSparseSetResolver.UnSub(PubSimpleMessageSparseSetResolverDelegate);
            return value;
        }
    }
}