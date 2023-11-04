using Common;
using UnityEngine;

namespace UI.WindowsSystem.WindowTypes
{
    public class InGameUI : WindowBase
    {
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.P)) return;

            GameContainer.Common.Resolve<WindowsSystem>().CreateWindow<GamePause>();
        }
    }
}