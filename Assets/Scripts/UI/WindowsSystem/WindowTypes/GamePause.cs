﻿using Common.DI;
using Cysharp.Threading.Tasks;
using GameCore.Camera;
using Networking.Client;
using Networking.Dataframes.InGame;
using Startup;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class GamePause : WindowBase
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _backToMenuButton;
        [SerializeField] private Button _settingsButton;

        [Inject] private GameCamera _gameCamera;
        [Inject] private WindowsSystem _windowsSystem;
        [Inject] private GameClientData _gameClientData;
        [Inject] private IGameClient _gameClient;
        [Inject] private GameInitializer _gameInitializer;

        private void Start()
        {
            _gameCamera.FollowTarget.SetInPause(true);
            
            _continueButton.onClick.AddListener(() =>
            {
                _gameCamera.FollowTarget.SetInPause(false);
                _windowsSystem.DestroyWindow(this);
            });
            
            _backToMenuButton.onClick.AddListener(() =>
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
                
                _windowsSystem.DestroyWindow(this);
                _gameInitializer.StopGame().Forget();
            });
            
            _settingsButton.onClick.AddListener(OpenSettings);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _gameCamera.FollowTarget.SetInPause(false);
                _windowsSystem.DestroyWindow(this);
            }
        }

        private void OpenSettings()
        {
            _windowsSystem.CreateWindow<SettingsScreen>();
        }
    }
}