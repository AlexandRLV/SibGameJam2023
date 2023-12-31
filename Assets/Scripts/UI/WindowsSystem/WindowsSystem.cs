﻿using System;
using System.Collections.Generic;
using Common.DI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UI.WindowsSystem
{
    public class WindowsSystem
    {
        private class NamedWindowContainer
        {
            public Type Type;
            public WindowBase Window;
        }
        
        private GameWindows _gameWindows;
        private UIRoot _uiRoot;
        
        private Dictionary<Type, WindowBase> _windowsPrefabs;
        private Dictionary<string, NamedWindowContainer> _namedWindowsPrefabs;
        private Dictionary<Type, WindowBase> _loadedWindows;
        
        [Construct]
        public WindowsSystem(GameWindows gameWindows, UIRoot uiRoot)
        {
            _gameWindows = gameWindows;
            _uiRoot = uiRoot;
            _windowsPrefabs = new Dictionary<Type, WindowBase>();
            _namedWindowsPrefabs = new Dictionary<string, NamedWindowContainer>();
            _loadedWindows = new Dictionary<Type, WindowBase>();

            foreach (var window in _gameWindows.windows)
            {
                _windowsPrefabs.Add(window.GetType(), window);
            }

            foreach (var namedContainer in _gameWindows.namedWindows)
            {
                _namedWindowsPrefabs.Add(namedContainer.WindowName, new NamedWindowContainer
                {
                    Type = namedContainer.Window.GetType(),
                    Window = namedContainer.Window,
                });
            }
        }

        public bool TryGetWindow<T>(out T window) where T : WindowBase
        {
            window = null;
            var type = typeof(T);
            if (_loadedWindows.TryGetValue(type, out var baseWindow))
            {
                if (baseWindow is not T targetWindow)
                    throw new ArgumentException($"Error in getting window type {type.Name} - have cached wrong type of window");

                window = targetWindow;
                return true;
            }

            return false;
        }

        public T CreateWindow<T>() where T : WindowBase
        {
            var type = typeof(T);
            if (_loadedWindows.TryGetValue(type, out var baseWindow))
            {
                if (baseWindow is not T targetWindow)
                    throw new ArgumentException($"Error in creating window type {type.Name} - already created wrong type of window");

                return targetWindow;
            }
            
            if (!_windowsPrefabs.TryGetValue(type, out var windowPrefabBase))
                throw new ArgumentException($"Error in getting window type {type.Name} - window not registered");
            
            if (windowPrefabBase is not T windowPrefab)
                throw new ArgumentException($"Error in getting window type {type.Name} - registered wrong window");
            
            var window = GameContainer.InstantiateAndResolve(windowPrefab, _uiRoot.WindowsParent);
            _loadedWindows.Add(type, window);
            return window;
        }

        public T CreateNamedWindow<T>(string name) where T : WindowBase
        {
            var type = typeof(T);
            if (_loadedWindows.TryGetValue(type, out var baseWindow))
            {
                if (baseWindow is not T targetWindow)
                    throw new ArgumentException($"Error in creating window type {type.Name} - already created wrong type of window");

                return targetWindow;
            }
            
            if (!_namedWindowsPrefabs.TryGetValue(name, out var container))
                throw new ArgumentException($"Error in getting window named {name} - window name not registered");
            
            if (container.Window is not T windowPrefab)
                throw new ArgumentException($"Error in getting window type {type.Name} with name {name} - registered wrong window");
            
            var window = GameContainer.InstantiateAndResolve(windowPrefab, _uiRoot.WindowsParent);
            _loadedWindows.Add(type, window);
            return window;
        }

        public void DestroyWindow<T>() where T : WindowBase
        {
            var type = typeof(T);
            if (!_loadedWindows.TryGetValue(type, out var window))
                return;
            
            if (window != null && window.gameObject != null)
                Object.Destroy(window.gameObject);
            
            _loadedWindows.Remove(type);
        }

        public void DestroyWindow<T>(T window) where T : WindowBase
        {
            var type = typeof(T);
            if (!_loadedWindows.TryGetValue(type, out var loadedWindow))
            {
                Debug.Log("Don't have this type of window");
                return;
            }

            if (loadedWindow != window)
                throw new ArgumentException($"Trying to destroy {type.Name} window, but saved different object!");
            
            if (loadedWindow != null && loadedWindow.gameObject != null)
                Object.Destroy(loadedWindow.gameObject);
            
            _loadedWindows.Remove(type);
        }

        public void DestroyAll()
        {
            foreach (var loadedWindow in _loadedWindows)
            {
                if (loadedWindow.Value != null && loadedWindow.Value.gameObject != null)
                    Object.Destroy(loadedWindow.Value.gameObject);
            }
            
            _loadedWindows.Clear();
        }
    }
}