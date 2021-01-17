using UnityEngine;

public interface IHandler<T>
{
    void Pub(T message);
    void Pub<HTarget>(T message);
    void Pub(T message, object target);
    void Sub(System.Delegate callback);
    void UnSub(System.Delegate callback);
}