using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.WindowsSystem
{
    [Serializable]
    public class WindowNameContainer
    {
        [SerializeField] public string WindowName;
        [SerializeField] public WindowBase Window;
    }
    
    [CreateAssetMenu(fileName = "Game Windows")]
    public class GameWindows : ScriptableObject
    {
        [SerializeField] public List<WindowBase> windows;
        [SerializeField] public List<WindowNameContainer> namedWindows;
    }
}