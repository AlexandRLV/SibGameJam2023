using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace UI.WindowsSystem
{
    public class WindowsSystem
    {
        private GameWindows _gameWindows;
        private UIRoot _uiRoot;
        
        private Dictionary<Type, WindowBase> _windowsPrefabs;
        private Dictionary<Type, WindowBase> _loadedWindows;
        
        public void Initialize(GameWindows gameWindows, UIRoot uiRoot)
        {
            _gameWindows = gameWindows;
            _uiRoot = uiRoot;
            _windowsPrefabs = new Dictionary<Type, WindowBase>();
            _loadedWindows = new Dictionary<Type, WindowBase>();

            foreach (var window in _gameWindows.windows)
            {
                _windowsPrefabs.Add(window.GetType(), window);
            }
        }

        public T GetWindow<T>() where T : WindowBase
        {
            var type = typeof(T);
            if (_loadedWindows.TryGetValue(type, out var baseWindow))
            {
                if (baseWindow is not T targetWindow)
                    throw new ArgumentException($"Error in getting window type {type.Name} - created wrong type of window");

                return targetWindow;
            }
            
            if (!_windowsPrefabs.TryGetValue(type, out var windowPrefabBase))
                throw new ArgumentException($"Error in getting window type {type.Name} - window not registered");
            
            if (windowPrefabBase is not T windowPrefab)
                throw new ArgumentException($"Error in getting window type {type.Name} - registered wrong window");
            
            var window = Object.Instantiate(windowPrefab, _uiRoot.WindowsParent);
            _loadedWindows.Add(type, window);
            return window;
        }

        public void DestroyWindow<T>() where T : WindowBase
        {
            var type = typeof(T);
            if (!_loadedWindows.TryGetValue(type, out var window))
                return;
            
            Object.Destroy(window.gameObject);
        }
    }
}