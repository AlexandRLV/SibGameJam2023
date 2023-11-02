namespace Startup
{
    public interface IInitializer
    {
        public void Initialize();
        public void Dispose();
    }
}