using System.Collections.Generic;

namespace Refsa.EventBus
{
    public static class GlobalEventBus
    {
        public static readonly EventBus EventBus;

        static GlobalEventBus()
        {
            EventBus = new EventBus();
        }
    }
}