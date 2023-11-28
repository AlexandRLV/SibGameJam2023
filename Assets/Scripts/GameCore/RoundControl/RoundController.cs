using Common.DI;
using GameCore.Common.Messages;
using GameCore.Enemies;
using GameCore.Player;
using GameCore.Sounds;
using LocalMessages;
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

        [Inject] private LocalMessageBroker _messageBroker;
        [Inject] private IPlayer _player;
        [Inject] private SoundService _soundService;
        [Inject] private GameClientData _gameClientData;
        [Inject] private IGameClient _gameClient;
        
        private LoseGameReason _loseGameReason;

        private void Start()
        {
            Timer = _settings.RoundLengthSeconds;
            Stage = RoundStage.Game;
            
            _messageBroker.Subscribe<PlayerDetectedMessage>(OnPlayerDetected);
            _messageBroker.Subscribe<PlayerEvacuatedMessage>(OnPlayerEvacuated);
            _messageBroker.Subscribe<PlayerDeadMessage>(OnPlayerDead);
            _messageBroker.Subscribe<ChangeCharacterMessage>(OnChangeCharacter);

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
            Debug.Log($"Losing game in round controller by reason: {reason}");
            Timer = _settings.playerDetectedToLoseSeconds;
            Stage = RoundStage.WaitToLose;
            _player.Unposess();
            _loseGameReason = reason;
            
            if (!send) return;
            if (!_gameClientData.IsConnected) return;

            Debug.Log("Sending lose game dataframe");
            var dataframe = new LoseGameDataframe
            {
                reason = (byte)reason
            };
            _gameClient.Send(ref dataframe);
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