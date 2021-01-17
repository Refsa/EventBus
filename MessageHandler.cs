using System;
using System.Linq;

public class MessageHandler<MData, MType> : IHandler<MType>
{
    public event System.Action<MData> observers;

    public void Pub(MType message)
    {
        Pub(message, observers.Invoke);
    }

    public void Pub<HTarget>(MType message)
    {
        Pub(message, (m) =>
        {
            foreach (System.Action<MData> observer in observers.GetInvocationList()
                .Where(e => e.Target.GetType().FullName.Contains(typeof(HTarget).Name)))
            {
                observer.Invoke(m);
            }
        });
    }

    public void Pub(MType message, object target)
    {
        Pub(message, (m) =>
        {
            foreach (System.Action<MData> observer in observers.GetInvocationList()
                .Where(e => e.Target == target))
            {
                observer.Invoke(m);
            }
        });
    }

    void Pub(MType message, System.Action<MData> action)
    {
        if (observers == null)
        {
            return;
        }

        if (message is MData t)
        {
            action.Invoke(t);
            return;
        }
        throw new System.ArgumentException($"Message given to message handler is of wrong type\nShould be {typeof(MData)} but was {message.GetType()}");
    }

    public void Sub(Delegate callback)
    {
        if (callback is System.Action<MData> c)
        {
            observers += c;
        }
    }

    public void UnSub(Delegate callback)
    {
        if (callback is System.Action<MData> c)
        {
            observers -= c;
        }
    }
}