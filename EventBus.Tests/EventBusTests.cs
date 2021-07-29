using System;
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
            var msg = new TestMessage{Content = 1234};
            var bus = new EventBus();

            int received = 0;
            bus.Sub<TestMessage>((in TestMessage m) => {
                received = m.Content;
            });

            bus.Pub(msg);

            Assert.Equal(msg.Content, received);
        }
    }
}
