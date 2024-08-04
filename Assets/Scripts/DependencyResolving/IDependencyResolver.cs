using System;

namespace Match3.DependencyResolving
{
    public interface IDependencyResolver : IDisposable
    {
        T Resolve<T>();

        void Add<TAbstraction, TImplementation>(TImplementation implementation)
            where TImplementation : class, TAbstraction;
    }
}

