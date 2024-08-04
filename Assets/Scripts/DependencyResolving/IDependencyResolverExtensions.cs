namespace Match3.DependencyResolving
{
    public static class IDependencyResolverExtensions
    {
        public static void Add<T>(this IDependencyResolver resolver, T dependency)
            where T : class
        {
            resolver.Add<T, T>(dependency);
        }
    }
}