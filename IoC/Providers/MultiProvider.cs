using System;

namespace Svelto.IoC
{
    public class MultiProvider<T>:IProvider<T> where T:new()
	{
        public object Create(Type containerContract)
        {
            return new T();
        }

        public Type contract { get { return typeof(T); } }
        public bool single   { get { return false; } }
	}
}

