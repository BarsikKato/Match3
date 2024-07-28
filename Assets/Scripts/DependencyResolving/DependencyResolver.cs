using System;
using System.Collections.Generic;

namespace DependencyResolving
{
    public sealed class DependencyResolver : IDependencyResolver
    {
        private static IDependencyResolver _instance;

        private readonly Dictionary<Type, object> _dependenciesByType;

        private DependencyResolver()
        {
            _dependenciesByType = new Dictionary<Type, object>();
        }

        static DependencyResolver()
        {
            _instance = new DependencyResolver();
        }

        public static IDependencyResolver Get() =>
            _instance;

        public void Add<TAbstraction, TImplementation>(TImplementation implementation)
            where TImplementation : class, TAbstraction
        {
            _dependenciesByType.Add(typeof(TAbstraction), implementation);
        }

        public T Resolve<T>()
        {
            return (T)_dependenciesByType[typeof(T)];
        }

        public void Dispose()
        {
            _dependenciesByType.Clear();
        }
    }
}
