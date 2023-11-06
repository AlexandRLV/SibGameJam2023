using Common;
using GameCore.Camera;
using GameCore.Common;
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
                LoseGameReason.Catched => "Тебя поймали!",
                LoseGameReason.Dead => "Ты погиб!"
            };
        }
    }
}