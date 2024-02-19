using System;
using Common.DI;
using GameCore.Camera;
using GameCore.Common;
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

        [Inject] private GameCamera _gameCamera;
        [Inject] private GameInitializer _gameInitializer;
        [Inject] private GameClientData _gameClientData;
        [Inject] private IGameClient _gameClient;
        
        private void Start()
        {
            if (_gameClientData.IsConnected)
            {
                var dataframe = new GameFinishedDataframe
                {
                    reason = GameFinishedReason.Lose
                };
                _gameClient.Send(ref dataframe);
                return;
            }
            
            _gameCamera.FollowTarget.SetInPause(true);
            _restartButton.onClick.AddListener(() =>
            {
                _gameInitializer.RestartGame();
            });
            
            _menuButton.onClick.AddListener(() =>
            {
                _gameInitializer.StopGame();
            });
        }

        public void Initialize(LoseGameReason reason)
        {
            _reasonLabel.text = reason switch
            {
                LoseGameReason.TimeOut => "Время вышло!",
                LoseGameReason.Catched => "Агента поймали!",
                LoseGameReason.Dead => "Агент погиб!",
                _ => "Агента поймали!"
            };
        }
    }
}