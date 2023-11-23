﻿using Common;
using GameCore.Common.Messages;
using GameCore.Player;
using GameCore.Sounds;
using LocalMessages;
using NetFrame.Client;
using Networking;
using Networking.Dataframes.InGame;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;
using UnityEngine;

namespace GameCore.Common
{
    public class RoundController : MonoBehaviour
    {
        public float Timer { get; private set; }

        public RoundStage Stage { get; set; }
        public RoundSettings settings => _settings;

        [SerializeField] private RoundSettings _settings;

        private LocalMessageBroker _messageBroker;
        private IPlayer _player;
        private SoundService _soundService;
        
        private LoseGameReason _loseGameReason;
        
        private GameClient _gameClient;
        private NetFrameClient _client;

        private void Start()
        {
            _soundService = GameContainer.Common.Resolve<SoundService>();
            
            Timer = _settings.RoundLengthSeconds;
            Stage = RoundStage.Game;
            
            _gameClient = GameContainer.Common.Resolve<GameClient>();
            _client = GameContainer.Common.Resolve<NetFrameClient>();

            _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            _messageBroker.Subscribe<PlayerDetectedMessage>(OnPlayerDetected);
            _messageBroker.Subscribe<PlayerEvacuatedMessage>(OnPlayerEvacuated);
            _messageBroker.Subscribe<PlayerDeadMessage>(OnPlayerDead);
            _messageBroker.Subscribe<ChangeCharacterMessage>(OnChangeCharacter);

            _player = GameContainer.InGame.Resolve<IPlayer>();
            _soundService.PlayMusic(_player.MouseType == PlayerMouseType.ThinMouse ? MusicType.ThinCharacter : MusicType.FatCharacter);
        }

        private void OnDestroy()
        {
            _messageBroker.Unsubscribe<PlayerDetectedMessage>(OnPlayerDetected);
            _messageBroker.Unsubscribe<PlayerEvacuatedMessage>(OnPlayerEvacuated);
            _messageBroker.Unsubscribe<PlayerDeadMessage>(OnPlayerDead);
            _messageBroker.Unsubscribe<ChangeCharacterMessage>(OnChangeCharacter);
        }

        public void LoseGameByReason(LoseGameReason reason, bool send = true)
        {
            Timer = _settings.playerDetectedToLoseSeconds;
            Stage = RoundStage.WaitToLose;
            _player.Unposess();
            _loseGameReason = reason;
            
            if (!send) return;
            if (!_gameClient.IsConnected) return;

            var dataframe = new LoseGameDataframe
            {
                reason = reason
            };
            _client.Send(ref dataframe);
        }

        private void OnChangeCharacter(ref ChangeCharacterMessage message)
        {
            _soundService.PlayMusic(_player.MouseType == PlayerMouseType.ThinMouse ? MusicType.ThinCharacter : MusicType.FatCharacter, true);
        }

        private void Update()
        {
            if (Stage == RoundStage.None) return;
            
            Timer -= Time.deltaTime;
            if (Timer > 0f) return;

            if (Stage == RoundStage.Game)
            {
                LoseGameByReason(LoseGameReason.TimeOut);
            }
            else if (Stage == RoundStage.WaitToLose)
            {
                LoseGame();
            }
        }

        private void LoseGame()
        {
            _soundService.StopSound();
            _soundService.PlayMusic(MusicType.Lose);
            Stage = RoundStage.None;
            _player.Unposess();

            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            var loseGameWindow = windowsSystem.CreateWindow<LoseScreen>();
            loseGameWindow.Initialize(_loseGameReason);
        }

        private void OnPlayerDetected(ref PlayerDetectedMessage value)
        {
            LoseGameByReason(LoseGameReason.Catched);
        }

        private void OnPlayerDead(ref PlayerDeadMessage value)
        {
            LoseGameByReason(LoseGameReason.Dead);
        }

        private void OnPlayerEvacuated(ref PlayerEvacuatedMessage value)
        {
            _soundService.StopSound();
            _soundService.PlayMusic(MusicType.Win);
            Stage = RoundStage.None;
            _player.Unposess();

            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateWindow<WinScreen>();
        }
    }
}