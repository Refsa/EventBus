public interface IOverrideBusLock { }
public struct LockBusMessage : IMessage, IOverrideBusLock { }
public struct UnlockBusMessage : IMessage, IOverrideBusLock { }

public struct BusLock
{
    public System.Action Callback;
    public IMessage SentMessage;
    public bool Locked;

    public static BusLock Message(System.Action callback, IMessage message)
    {
        return new BusLock
        {
            Locked = false,
            SentMessage = message,
            Callback = callback
        };
    }

    public static BusLock Lock()
    {
        return new BusLock
        {
            Locked = true,
            Callback = null
        };
    }
}