using System;
using System.Reflection;

namespace Svelto.IoC
{
    class StandardProvider<T> : IProvider<T> where T : new()
    {
        public StandardProvider()
        {
            _type = typeof(T);
        }
        public bool Create(Type containerContract, PropertyInfo info, out object instance)
        {
            bool mustInject = false; 

            if (_object == null)
            {
                _object = new T();
                mustInject = true;
            }

            instance = _object;

            return mustInject;
        }

        public Type contract { get { return _type; } }

        T _object;
        Type _type;
    }
}
