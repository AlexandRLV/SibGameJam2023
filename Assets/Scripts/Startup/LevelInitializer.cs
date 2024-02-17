using System.Collections.Generic;
using Common.DI;
using UnityEngine;

namespace Startup
{
    public class LevelInitializer : MonoBehaviour
    {
        [SerializeField] private List<InitializerBase> _initializers;

        private void Awake()
        {
            foreach (var initializer in _initializers)
            {
                var type = initializer.GetType();
                Debug.Log($"Initializing level initializer {type.Name}");
                
                GameContainer.InjectToInstance(initializer);
                initializer.Initialize();
            }
        }

        private void OnDestroy()
        {
            foreach (var initializer in _initializers)
            {
                initializer.Dispose();
            }
        }
    }
}