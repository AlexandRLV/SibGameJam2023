using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Startup
{
    // TODO: switch to new initializers
    public abstract class BaseInitializer : MonoBehaviour
    {
        public abstract UniTask Initialize();
    }
}