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
        MessageBus messageBusGlobalResolver;
        MessageBus messageBusStaticResolver;
        MessageHandlerDelegates.MessageHandler<SimpleMessage> PubSimpleMessageDictResolverDelegate;
        MessageHandlerDelegates.MessageHandler<SimpleMessage> PubSimpleMessageSparseSetResolverDelegate;
        MessageHandlerDelegates.MessageHandler<SimpleMessage> PubSimpleMessageGlobalResolverDelegate;

        public MessageBusBench()
        {
            messageBusDictResolver = new MessageBus(new DictionaryResolver());
            messageBusSparseSetResolver = new MessageBus(new SparseSetResolver());
            messageBusGlobalResolver = new MessageBus(new GlobalResolver());
            messageBusStaticResolver = new MessageBus(new StaticResolver());
        }

        [Benchmark]
        public int PubSimpleMessageDictResolver_10M()
        {
            int value = 0;
            PubSimpleMessageDictResolverDelegate = (in SimpleMessage msg) =>
            {
                value++;
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
                value++;
            };
            messageBusSparseSetResolver.Sub(PubSimpleMessageSparseSetResolverDelegate);

            for (int i = 0; i < 10_000_000; i++)
            {
                messageBusSparseSetResolver.Pub(new SimpleMessage { Value = 1234 });
            }

            messageBusSparseSetResolver.UnSub(PubSimpleMessageSparseSetResolverDelegate);
            return value;
        }

        [Benchmark]
        public int PubSimpleMessageGlobalResolver_10M()
        {
            int value = 0;
            PubSimpleMessageGlobalResolverDelegate = (in SimpleMessage msg) =>
            {
                value++;
            };
            messageBusGlobalResolver.Sub(PubSimpleMessageGlobalResolverDelegate);

            for (int i = 0; i < 10_000_000; i++)
            {
                messageBusGlobalResolver.Pub(new SimpleMessage { Value = 1234 });
            }

            messageBusGlobalResolver.UnSub(PubSimpleMessageGlobalResolverDelegate);
            return value;
        }

        [Benchmark]
        public int PubSimpleMessageStaticResolver_10M()
        {
            int value = 0;
            MessageHandlerDelegates.MessageHandler<SimpleMessage> callback = (in SimpleMessage msg) =>
            {
                value++;
            };
            messageBusStaticResolver.Sub(callback);

            for (int i = 0; i < 10_000_000; i++)
            {
                messageBusStaticResolver.Pub(new SimpleMessage { Value = 1234 });
            }

            messageBusStaticResolver.UnSub(callback);
            return value;
        }
    }
}