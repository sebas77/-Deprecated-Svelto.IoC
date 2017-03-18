using System;
using System.Collections.Generic;
using System.Reflection;

namespace Svelto.IoC
{
    class NamedProvider<T> : IProvider<T> where T : new()
    {
        Dictionary<string, T> namedInjection = new Dictionary<string, T>();

        public bool Create(Type containerContract, PropertyInfo info, out object instance)
        {
            T returnObj;

            InjectAttribute[] attributes = (InjectAttribute[]) info.GetCustomAttributes(typeof(InjectAttribute), false);
            var name = attributes[0].name;

            bool mustInject = false;
            if (String.IsNullOrEmpty(name) == false)
            {
                if (namedInjection.TryGetValue(name, out returnObj) == false)
                {
                    returnObj = namedInjection[name] = new T();
                    mustInject = true;
                }
            }
            else
            {
                if (sharedObject == null)
                {
                    sharedObject = new T();
                    mustInject = true;
                }

                returnObj = sharedObject;
            }

            instance = returnObj;

            return mustInject;
        }

        public T sharedObject;

        public Type contract
        {
            get { return typeof(T); }
        }
    }
}