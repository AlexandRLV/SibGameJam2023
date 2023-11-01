using Cysharp.Threading.Tasks;

namespace Startup
{
    public interface IInitializer
    {
        public UniTask Initialize();
        public void Dispose();
    }
}