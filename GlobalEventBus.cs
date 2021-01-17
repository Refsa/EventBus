
using System.Collections.Generic;

namespace Refsa.EventBus
{
    public class GlobalEventBus
    {
        static GlobalEventBus instance;
        public static GlobalEventBus Instance => instance;

        EventBus eventBus;
        public static Stack<BusLock> IncomingMessages => instance.eventBus.IncomingMessages;

        public static EventBus Bus => instance.eventBus;

        static GlobalEventBus()
        {
            if (instance == null) instance = new GlobalEventBus();
            else return;
        }

        GlobalEventBus()
        {
            eventBus = new EventBus();
        }
    }
}