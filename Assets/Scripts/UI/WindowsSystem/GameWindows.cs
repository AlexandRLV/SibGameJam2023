using System.Collections.Generic;
using UnityEngine;

namespace UI.WindowsSystem
{
    [CreateAssetMenu(fileName = "Game Windows")]
    public class GameWindows : ScriptableObject
    {
        [SerializeField] public List<WindowBase> windows;
    }
}