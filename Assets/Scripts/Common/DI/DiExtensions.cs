namespace Common.DI
{
    public static class DiExtensions
    {
        public static bool CanResolve<T>(this Container container) =>
            container != null && container.HasRegistration<T>();
    }
}