using System;
using System.Linq;
using Xunit;
using Refsa.EventBus;

namespace Refsa.EventBus.Tests
{
    public class MessagePipeTests
    {
        struct TestMessage : IMessage
        {
            public int Value;
        }

        [Fact]
        public void message_pipe_down()
        {
            MessagePipe root = new MessagePipe();
            MessagePipe left = new MessagePipe(root);
            MessagePipe right = new MessagePipe(root);

            bool received = false;
            right.Sub<TestMessage>((in TestMessage tm) => { received = true; });

            root.PubDown(new TestMessage());

            Assert.True(received);
        }

        [Fact]
        public void message_pipe_up()
        {
            MessagePipe root = new MessagePipe();
            MessagePipe left = new MessagePipe(root);
            MessagePipe right = new MessagePipe(root);

            bool received = false;
            root.Sub<TestMessage>((in TestMessage tm) => { received = true; });

            right.PubUp(new TestMessage());

            Assert.True(received);
        }

        [Fact]
        public void message_pip_side()
        {
            MessagePipe root = new MessagePipe();
            MessagePipe left = new MessagePipe(root);
            MessagePipe right = new MessagePipe(root);
            MessagePipe right_left = new MessagePipe(right);

            bool received = false;
            right_left.Sub((in TestMessage ts) => received = true);

            bool recv_left = false;
            left.Sub((in TestMessage ts) => recv_left = true);

            left.PubSide(new TestMessage());
            Assert.True(received);
            Assert.False(recv_left);
        }
    }
}