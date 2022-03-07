using BenchmarkDotNet;
using BenchmarkDotNet.Running;

namespace Refsa.EventBus.Bench;

static class Program
{
    static void Main()
    {
        var summary = BenchmarkRunner.Run<MessageBusBench>();
    }
}