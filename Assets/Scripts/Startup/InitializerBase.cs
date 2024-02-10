using UnityEngine;

namespace Startup
{
    public abstract class InitializerBase : MonoBehaviour
    {
        public abstract void Initialize();
        public abstract void Dispose();
    }
}