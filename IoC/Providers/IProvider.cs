using System;

namespace Svelto.IoC
{
    public interface IProvider
    {
        object Create(Type containerContract);

        Type contract { get; }
        bool single { get; }
    }

    public interface IProvider<T> : IProvider
    { }
}