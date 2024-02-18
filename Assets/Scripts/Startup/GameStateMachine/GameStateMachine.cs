﻿using System.Collections.Generic;
using Common.DI;
using Startup.GameStateMachine.States;
using UnityEngine;

namespace Startup.GameStateMachine
{
    public enum GameStateType
    {
        Menu,
        Game,
        MultiplayerGame
    }
    
    public class GameStateMachine
    {
        private readonly Dictionary<GameStateType, IGameState> _states = new()
        {
            { GameStateType.Menu, GameContainer.Create<MainMenuGameState>() },
            { GameStateType.Game, GameContainer.Create<PlayGameState>() },
        };

        private GameStateType _currentStateType;
        private IGameState _currentState;

        public void SwitchToState(GameStateType targetState, bool force = false)
        {
            Debug.Log($"Switching to state {targetState}");
            if (_currentStateType == targetState && !force)
            {
                Debug.Log($"Already in state {targetState}");
                return;
            }

            _currentState?.OnExit();

            _currentStateType = targetState;
            _currentState = _states[targetState];
            
            _currentState.OnEnter();
        }
    }
}