using System;
using System.Reflection;

namespace Svelto.IoC
{
    public interface IProvider
    {
        bool Create(Type containerContract, PropertyInfo info, out object instance);

        Type contract { get; }
    }

    public interface IProvider<T> : IProvider
    { }
}
