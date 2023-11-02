﻿using Common;
using LocalMessages;
using Startup;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public struct StartGameMessage
    {
        public int X;
        public string Text;
    }
    
    public class MainMenu : WindowBase
    {
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _leaveButton;

        private GameInitializer _gameInitializer;
        
        private void Awake()
        {
            _gameInitializer = GameContainer.Common.Resolve<GameInitializer>();
            
            _startGameButton.onClick.AddListener(StartGame);
            _leaveButton.onClick.AddListener(LeaveGame);
        }

        private async void StartGame()
        {
            Hide();
            await _gameInitializer.StartGame();
            Destroy(gameObject);
        }

        private void LeaveGame()
        {
            Application.Quit();
        }
    }
}