using System;
using System.Reflection;

namespace Svelto.IoC
{
    class MultiProvider<T>:IProvider<T> where T:new()
	{
        public MultiProvider()
        {
            _type = typeof(T);
        }
        public bool Create(Type containerContract, PropertyInfo info, out object instance)
        {
            instance = new T();

            return true;
        }

        public Type contract { get { return _type; } }

        Type    _type;
	}
}

