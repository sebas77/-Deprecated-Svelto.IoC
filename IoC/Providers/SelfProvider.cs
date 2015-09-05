using System;

namespace Svelto.IoC
{
    public class SelfProvider<T> : IProvider<T>
    {
        public SelfProvider(T instance)
        {
            _instance = instance;
        }

        public object Create(Type containerContract)
        {
            return _instance;
        }

        public Type contract { get { return typeof(T); } }
        public bool single { get { return true; } }

        T _instance; //should it be weak reference?
    }
}
