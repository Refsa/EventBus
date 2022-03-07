using System;
using System.Linq;
using Xunit;
using Refsa.EventBus;
using System.Reflection;

namespace Refsa.EventBus.Tests
{
    public class ResolverTests
    {
        struct TestMessage1 : IMessage { }
        struct TestMessage2 : IMessage { }

        static readonly FieldInfo sparseSetFI = typeof(SparseSetResolver).GetField("indices", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo sparseSetCountFI = typeof(SparseSet).GetField("count", BindingFlags.NonPublic | BindingFlags.Instance);

        [Fact]
        public void sparse_set_resolver_add()
        {
            var resolver = new SparseSetResolver();
            var sparseSet = sparseSetFI.GetValue(resolver);
            int count = 0;

            for (int i = 0; i < 100; i++)
            {
                resolver.GetHandler<TestMessage1>();
                count = (int)sparseSetCountFI.GetValue(sparseSet);
                Assert.Equal(1, count);
            }

            resolver.GetHandler<TestMessage2>();
            count = (int)sparseSetCountFI.GetValue(sparseSet);
            Assert.Equal(2, count);
        }

        static readonly FieldInfo resolverIndexFI = typeof(StaticResolver).GetField("resolverIndex", BindingFlags.NonPublic | BindingFlags.Instance);

        void add_static_resolver()
        {
            var resolver2 = new StaticResolver();
            Assert.Equal(1, (int)resolverIndexFI.GetValue(resolver2));
        }

        [Fact]
        public void static_resolver_add()
        {
            var resolver1 = new StaticResolver();

            Assert.Equal(0, (int)resolverIndexFI.GetValue(resolver1));

            add_static_resolver();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            var resolver3 = new StaticResolver();
            Assert.Equal(1, (int)resolverIndexFI.GetValue(resolver3));
        }
    }
}