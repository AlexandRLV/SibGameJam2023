using Common;
using GameCore.Camera;
using GameCore.Common;
using Startup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class WinScreen : WindowBase
    {
        [SerializeField] private Button _menuButton;

        private void Start()
        {
            GameContainer.InGame.Resolve<GameCamera>().FollowTarget.SetInPause(true);
            _menuButton.onClick.AddListener(() =>
            {
                GameContainer.Common.Resolve<GameInitializer>().StopGame();
            });
        }
    }
}