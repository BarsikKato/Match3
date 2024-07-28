using System;

namespace DependencyResolving
{
    public interface IDependencyResolver : IDisposable
    {
        T Resolve<T>();

        void Add<TAbstraction, TImplementation>(TImplementation implementation)
            where TImplementation : class, TAbstraction;
    }
}

