using System;
using System.Reflection;

namespace Svelto.IoC
{
    class SelfProvider<T> : IProvider<T>
    {
        public SelfProvider(T instance)
        {
            _instance = instance;
            _type = typeof(T);
        }

        public bool Create(Type containerContract, PropertyInfo info, out object instance)
        {
            instance = _instance;

            if (_mustBeInjected == true)
            {
                _mustBeInjected = false;

                return true;
            }

            return false;
        }

        public Type contract { get { return _type; } }

        T       _instance;
        bool    _mustBeInjected = true;
        Type    _type;
    }
}
