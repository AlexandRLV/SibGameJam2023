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
        
        public void Initialize(GameWindows gameWindows, UIRoot uiRoot)
        {
            _gameWindows = gameWindows;
            _uiRoot = uiRoot;
            _windowsPrefabs = new Dictionary<Type, WindowBase>();

            foreach (var window in _gameWindows.windows)
            {
                _windowsPrefabs.Add(window.GetType(), window);
            }
        }

        public T GetWindow<T>() where T : WindowBase
        {
            var type = typeof(T);
            if (!_windowsPrefabs.TryGetValue(type, out var windowPrefab))
                throw new ArgumentException($"Error in getting window type {type.Name} - window not registered");
            
            if (windowPrefab is not T window)
                throw new ArgumentException($"Error in getting window type {type.Name} - registered wrong window");
            
            return Object.Instantiate(window, _uiRoot.WindowsParent);
        }
    }
}