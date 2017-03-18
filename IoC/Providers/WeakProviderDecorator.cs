using System;
using System.Reflection;
using Svelto.DataStructures;

namespace Svelto.IoC
{
    class WeakProviderDecorator<T> : IProvider<T>
    {
        public WeakProviderDecorator(IProvider<T> provider)
        {
            _reference = new WeakReference<IProvider<T>>(provider);
        }
        public bool Create(Type containerContract, PropertyInfo info, out object instance)
        {
            if (_reference.IsValid)
                return _reference.Target.Create(containerContract, info, out instance);
            
            instance = null;
            return false;
        }

        public IProvider<T> provider { get { return _reference.Target; } }

        public Type contract { get { return _reference.IsValid ? _reference.Target.contract : null; } }

        WeakReference<IProvider<T>> _reference;
    }
}
