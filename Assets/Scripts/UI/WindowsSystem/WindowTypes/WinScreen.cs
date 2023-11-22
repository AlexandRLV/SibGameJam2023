using Common;
using GameCore.Camera;
using GameCore.Common;
using NetFrame.Client;
using Networking;
using Networking.Dataframes.InGame;
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
            if (GameContainer.Common.Resolve<GameClient>().IsConnected)
            {
                var dataframe = new GameFinishedDataframe
                {
                    reason = GameFinishedReason.Win
                };
                GameContainer.Common.Resolve<NetFrameClient>().Send(ref dataframe);
                return;
            }
            
            GameContainer.InGame.Resolve<GameCamera>().FollowTarget.SetInPause(true);
            _menuButton.onClick.AddListener(() =>
            {
                GameContainer.Common.Resolve<GameInitializer>().StopGame();
            });
        }
    }
}