using Common;
using GameCore.Common.Messages;
using GameCore.Player;
using GameCore.Sounds;
using LocalMessages;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;
using UnityEngine;

namespace GameCore.Common
{
    public class RoundController : MonoBehaviour
    {
        public float Timer { get; private set; }

        public RoundStage Stage { get; set; }

        [SerializeField] private RoundSettings _settings;

        private LocalMessageBroker _messageBroker;
        private TwoMousePlayer _player;
        private SoundService _soundService;
        
        private LoseGameReason _loseGameReason;

        private void Start()
        {
            _soundService = GameContainer.Common.Resolve<SoundService>();
            
            Timer = _settings.RoundLengthSeconds;
            Stage = RoundStage.Game;

            _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            _messageBroker.Subscribe<PlayerDetectedMessage>(OnPlayerDetected);
            _messageBroker.Subscribe<PlayerEvacuatedMessage>(OnPlayerEvacuated);
            _messageBroker.Subscribe<PlayerDeadMessage>(OnPlayerDead);

            _player = GameContainer.InGame.Resolve<TwoMousePlayer>();
            _soundService.PlayMusic(_player.MouseType == PlayerMouseType.ThinMouse ? MusicType.ThinCharacter : MusicType.FatCharacter);
        }

        private void OnDestroy()
        {
            _messageBroker.Unsubscribe<PlayerDetectedMessage>(OnPlayerDetected);
            _messageBroker.Unsubscribe<PlayerEvacuatedMessage>(OnPlayerEvacuated);
            _messageBroker.Unsubscribe<PlayerDeadMessage>(OnPlayerDead);
        }

        private void Update()
        {
            if (Stage == RoundStage.None) return;

            if (UnityEngine.Input.GetKeyDown(_settings.mouseChangeKey) &&
                Stage == RoundStage.Game)
            {
                _player.PosessAnother();
                
                var message = new ChangeCharacterMessage();
                message.isThinMouse = _player.MouseType == PlayerMouseType.ThinMouse;
                _messageBroker.Trigger(ref message);
                
                _soundService.PlayMusic(_player.MouseType == PlayerMouseType.ThinMouse ? MusicType.ThinCharacter : MusicType.FatCharacter);
            }
            
            Timer -= Time.deltaTime;
            if (Timer > 0f) return;

            if (Stage == RoundStage.Game)
            {
                _loseGameReason = LoseGameReason.TimeOut;
                Timer = _settings.playerDetectedToLoseSeconds;
                Stage = RoundStage.WaitToLose;
                _player.UnposessAll();
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
            _player.UnposessAll();

            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            var loseGameWindow = windowsSystem.CreateWindow<LoseScreen>();
            loseGameWindow.Initialize(_loseGameReason);
        }

        private void OnPlayerDetected(ref PlayerDetectedMessage value)
        {
            Timer = _settings.playerDetectedToLoseSeconds;
            Stage = RoundStage.WaitToLose;
            _player.UnposessAll();
            _loseGameReason = LoseGameReason.Catched;
        }

        private void OnPlayerDead(ref PlayerDeadMessage value)
        {
            Timer = _settings.playerDetectedToLoseSeconds;
            Stage = RoundStage.WaitToLose;
            _player.UnposessAll();
            _loseGameReason = LoseGameReason.Dead;
        }

        private void OnPlayerEvacuated(ref PlayerEvacuatedMessage value)
        {
            _soundService.StopSound();
            _soundService.PlayMusic(MusicType.Win);
            Stage = RoundStage.None;
            _player.UnposessAll();

            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateWindow<WinScreen>();
        }
    }
}