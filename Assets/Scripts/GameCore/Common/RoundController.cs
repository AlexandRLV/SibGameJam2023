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

        public RoundStage Stage
        {
            get => _stage;
            private set
            {
                _stage = value;
                switch (value)
                {
                    case RoundStage.FatMouse:
                        _soundService.PlayMusic(MusicType.FatCharacter);
                        break;
                    case RoundStage.ThinMouse:
                        _soundService.PlayMusic(MusicType.ThinCharacter);
                        break;
                }
            }
        }

        [SerializeField] private RoundSettings _settings;

        private LocalMessageBroker _messageBroker;
        private GamePlayer _player;
        private SoundService _soundService;
        
        private LoseGameReason _loseGameReason;
        private RoundStage _stage;

        private void Start()
        {
            _soundService = GameContainer.Common.Resolve<SoundService>();
            
            Timer = _settings.RoundLengthSeconds;
            Stage = RoundStage.FatMouse;

            _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            _messageBroker.Subscribe<PlayerDetectedMessage>(OnPlayerDetected);
            _messageBroker.Subscribe<PlayerEvacuatedMessage>(OnPlayerEvacuated);
            _messageBroker.Subscribe<PlayerDeadMessage>(OnPlayerDead);

            _player = GameContainer.InGame.Resolve<GamePlayer>();
            _player.PosessFatMouse();
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
                Stage is RoundStage.FatMouse or RoundStage.ThinMouse)
            {
                _player.PosessAnother();
                
                var message = new ChangeCharacterMessage();
                message.isThinMouse = _player.IsThinMouse;
                _messageBroker.Trigger(ref message);
            }
            
            Timer -= Time.deltaTime;
            if (Timer > 0f) return;

            if (Stage == RoundStage.FatMouse)
            {
                var message = new ChangeCharacterMessage();
                message.isThinMouse = true;
                _messageBroker.Trigger(ref message);
                
                Timer = _settings.RoundLengthSeconds;
                Stage = RoundStage.ThinMouse;
                _player.PosessThinMouse();
            }
            else if (Stage == RoundStage.ThinMouse)
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