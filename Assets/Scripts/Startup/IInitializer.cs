using System.Collections;

namespace Startup
{
    public interface IInitializer
    {
        public IEnumerator Initialize();
        public void Dispose();
    }
}