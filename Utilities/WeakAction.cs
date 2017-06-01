using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Svelto.DataStructures;

public class WeakAction<T1, T2> : WeakAction
{
    public WeakAction(Action<T1, T2> listener)
        : base(listener.Target, listener.Method)
    {
    }

    public void Invoke(T1 data1, T2 data2)
    {
        if (ObjectRef.IsValid)
            Method.Invoke(ObjectRef.Target, new object[] { data1, data2 });
        else
            Utility.Console.LogWarning("Target of weak action has been garbage collected");
    }
}

public class WeakAction<T> : WeakActionBase
{
    public WeakAction(Action<T> listener)
        : base(listener.Target, listener.Method)
    {
    }

    public void Invoke(T data)
    {
        Invoke_Internal(data);
    }

    void Invoke_Internal(T data)
    {
        if (ObjectRef.IsValid)
            Method.Invoke(ObjectRef.Target, new object[] {data});
        else
            Utility.Console.LogWarning("Target of weak action has been garbage collected");
    }
}

public class WeakAction : WeakActionBase
{
    public WeakAction(Action listener) 
        : base(listener)
    {
    }

    public WeakAction(object listener, MethodInfo method) 
        : base(listener, method)
    {
    }
    
    public void Invoke()
    {
        if (ObjectRef.IsValid)
            Method.Invoke(ObjectRef.Target, null);
        else
            Utility.Console.LogError("Target of weak action has been garbage collected");
    }
}

public abstract class WeakActionBase
{
    protected readonly WeakReference<object> ObjectRef;
    protected readonly MethodInfo Method;
    
    public bool IsValid
    {
        get { return ObjectRef.IsValid; }
    }

    public WeakActionBase(Action listener)
        : this(listener.Target, listener.Method)
    { }

    public WeakActionBase(object listener, MethodInfo method)
    {
        ObjectRef = new WeakReference<object>(listener);
        
        Method = method;

        if (method.DeclaringType.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length != 0)
            throw new ArgumentException("Cannot create weak event to anonymous method with closure.");
    }
}

