using System;
using System.Linq;
using Xunit;
using Refsa.EventBus;

namespace Refsa.EventBus.Tests
{
    public class EventBusTests
    {
        struct TestMessage : IMessage
        {
            public int Content;
        }

        [Fact]
        public void sub_and_pub_message()
        {
            var msg = new TestMessage { Content = 1234 };
            var bus = new MessageBus();

            int received = 0;
            bus.Sub<TestMessage>((in TestMessage m) =>
            {
                received = m.Content;
            });

            bus.Pub(msg);

            Assert.Equal(msg.Content, received);
        }

        [Fact]
        public void unsub_message()
        {
            int counter = 0;

            var msg = new TestMessage();
            var bus = new MessageBus();

            bus.Sub<TestMessage>(MsgHandler);
            bus.Pub(msg);
            bus.UnSub<TestMessage>(MsgHandler);
            bus.Pub(msg);

            Assert.Equal(1, counter);

            void MsgHandler(in TestMessage msg)
            {
                counter++;
            }
        }

        [Fact]
        public void busses_dont_overlap()
        {
            var msg1 = new TestMessage { Content = 1234 };
            var msg2 = new TestMessage { Content = 4321 };

            var eb1 = new MessageBus();
            var eb2 = new MessageBus();

            int recv1 = 0;
            eb1.Sub<TestMessage>((in TestMessage m) => { recv1 = m.Content; });
            int recv2 = 0;
            eb2.Sub<TestMessage>((in TestMessage m) => { recv2 = m.Content; });

            eb1.Pub(msg1);
            eb2.Pub(msg2);

            Assert.Equal(msg1.Content, recv1);
            Assert.Equal(msg2.Content, recv2);
        }

        [Fact]
        public void event_bus_can_be_used_in_multiple_threads()
        {
            var msg1 = new TestMessage { Content = 1234 };
            var eb = new MessageBus();

            var recv = new System.Collections.Concurrent.ConcurrentBag<int>();
            eb.Sub<TestMessage>((in TestMessage m) => { recv.Add(m.Content); });

            int taskCount = 100;

            var startTime = DateTime.Now + TimeSpan.FromSeconds(2);
            var tasks = new System.Collections.Generic.List<System.Threading.Tasks.Task>();
            for (int i = 0; i < taskCount; i++)
            {
                int j = i;
                tasks.Add(System.Threading.Tasks.Task.Run(() =>
                {
                    while (DateTime.Now < startTime)
                    {
                        System.Threading.Thread.Sleep(1);
                    }

                    eb.Pub(new TestMessage { Content = j });
                }));
            }

            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

            var expected = new System.Collections.Generic.HashSet<int>();

            Assert.Equal(taskCount, recv.Count);
            while(recv.TryTake(out int value))
            {
                expected.Add(value);
            }

            Assert.Equal(taskCount, expected.Count);
            Assert.Equal(CalcSum(taskCount), expected.Sum());
        }

        int CalcSum(int to)
        {
            int sum = 0;
            for (int i = 0; i < to; i++)
            {
                sum += i;
            }
            return sum;
        }
    }
}
