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

        static readonly FieldInfo resolverIndexFI = typeof(SparseSetResolver).GetField("resolverIndex", BindingFlags.NonPublic | BindingFlags.Instance);

        void add_sparse_set_resolver()
        {
            var resolver2 = new SparseSetResolver();
            Assert.Equal(1, (int)resolverIndexFI.GetValue(resolver2));
        }

        [Fact]
        public void sparse_set_resolver_add()
        {
            var resolver1 = new SparseSetResolver();

            Assert.Equal(0, (int)resolverIndexFI.GetValue(resolver1));

            add_sparse_set_resolver();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            var resolver3 = new SparseSetResolver();
            Assert.Equal(1, (int)resolverIndexFI.GetValue(resolver3));
        }
    }
}