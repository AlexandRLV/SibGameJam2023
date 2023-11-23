using Common;
using Common.DI;
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
    public class LoseScreen : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _reasonLabel;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _menuButton;

        private void Start()
        {
            if (GameContainer.Common.Resolve<GameClient>().IsConnected)
            {
                var dataframe = new GameFinishedDataframe
                {
                    reason = GameFinishedReason.Lose
                };
                GameContainer.Common.Resolve<GameClient>().Send(ref dataframe);
                return;
            }
            
            GameContainer.InGame.Resolve<GameCamera>().FollowTarget.SetInPause(true);
            _restartButton.onClick.AddListener(() =>
            {
                GameContainer.Common.Resolve<GameInitializer>().RestartGame();
            });
            
            _menuButton.onClick.AddListener(() =>
            {
                GameContainer.Common.Resolve<GameInitializer>().StopGame();
            });
        }

        public void Initialize(LoseGameReason reason)
        {
            _reasonLabel.text = reason switch
            {
                LoseGameReason.TimeOut => "Время вышло!",
                LoseGameReason.Catched => "Агента поймали!",
                LoseGameReason.Dead => "Агент погиб!"
            };
        }
    }
}