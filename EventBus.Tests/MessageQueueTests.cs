using System;
using System.Linq;
using Xunit;
using Refsa.EventBus;

namespace Refsa.EventBus.Tests
{
    public class MessageQueueTests
    {
        struct TestMessage : IMessage, System.IEquatable<TestMessage>
        {
            public int Content;

            public bool Equals(TestMessage other)
            {
                return other.Content == Content;
            }
        }

        [Fact]
        public void message_bag_modify()
        {
            var bag = new MessageBag<TestMessage>(4);

            var msg = new TestMessage { Content = 1234 };
            bag.Enqueue(msg);

            Assert.True(bag.Dequeue(out var fromBag));
            Assert.True(msg.Equals(fromBag));
        }

        [Fact]
        public void message_queue_pub_single()
        {
            bool wasEqual = false;
            var msg = new TestMessage { Content = 1234 };

            var mq = new MessageQueue(4);
            mq.Bus.Sub<TestMessage>((in TestMessage pub) =>
            {
                wasEqual = msg.Equals(pub);
            });

            mq.Enqueue(msg);
            mq.DispatchSingle<TestMessage>();

            Assert.True(wasEqual);
        }

        [Fact]
        public void message_queue_pub_multi()
        {
            int testCount = 16;
            int pubCount = 0;

            var mq = new MessageQueue(testCount);
            mq.Bus.Sub<TestMessage>((in TestMessage pub) =>
            {
                Assert.Equal(pubCount, pub.Content);
                pubCount++;
            });

            for (int i = 0; i < testCount; i++)
            {
                mq.Enqueue(new TestMessage { Content = i });
            }

            mq.DispatchAll<TestMessage>();

            Assert.Equal(testCount, pubCount);
        }
    }
}