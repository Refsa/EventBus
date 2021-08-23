using System.Collections.Generic;

namespace Refsa.EventBus
{
    public static class GlobalEventBus
    {
        public static readonly MessageBus EventBus;

        static GlobalEventBus()
        {
            EventBus = new MessageBus();
        }
    }
}