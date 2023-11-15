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
        private SoundService SoundService => GameContainer.Common.Resolve<SoundService>();
        public float Timer { get; private set; }
        private RoundStage _stage;

        public RoundStage Stage
        {
            get => _stage;
            private set
            {
                _stage = value;
                switch (value)
                {
                    case RoundStage.FatMouse:
                        SoundService.PlayMusic(MusicType.FatCharacter);
                        break;
                    case RoundStage.ThinMouse:
                        SoundService.PlayMusic(MusicType.ThinCharacter);
                        break;
                }
            }
        }

        public RoundData Data { get; private set; }

        [SerializeField] private RoundSettings _settings;

        private LocalMessageBroker _messageBroker;
        private GamePlayer _player;
        private LoseGameReason _loseGameReason;
        
        private bool _evacuationActivated;

        private void Start()
        {
            Data = new RoundData();
            Timer = _settings.RoundLengthSeconds;
            Stage = RoundStage.FatMouse;
            _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            _messageBroker.Subscribe<PlayerDetectedMessage>(OnPlayerDetected);
            _messageBroker.Subscribe<PlayerWinMessage>(OnPlayerWin);
            _messageBroker.Subscribe<PlayerDeadMessage>(OnPlayerDead);

            _player = GameContainer.InGame.Resolve<GamePlayer>();
            _player.PosessFatMouse();
        }

        private void OnPlayerDead(ref PlayerDeadMessage value)
        {
            Timer = _settings.playerDetectedToLoseSeconds;
            Stage = RoundStage.WaitToLose;
            _player.UnposessAll();
            _loseGameReason = LoseGameReason.Dead;
        }

        private void OnPlayerWin(ref PlayerWinMessage value)
        {
            SoundService.StopSound();
            SoundService.PlayMusic(MusicType.Win);
            Stage = RoundStage.None;
            _player.UnposessAll();

            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateWindow<WinScreen>();
        }

        private void OnDestroy()
        {
            _messageBroker.Unsubscribe<PlayerDetectedMessage>(OnPlayerDetected);
            _messageBroker.Unsubscribe<PlayerWinMessage>(OnPlayerWin);
            _messageBroker.Unsubscribe<PlayerDeadMessage>(OnPlayerDead);
        }

        private void OnPlayerDetected(ref PlayerDetectedMessage value)
        {
            Timer = _settings.playerDetectedToLoseSeconds;
            Stage = RoundStage.WaitToLose;
            _player.UnposessAll();
            _loseGameReason = LoseGameReason.Catched;
        }

        public void CatchCactus()
        {
            Data.CactusCatched = true;
        }

        public void SaveMouse()
        {
            Data.MouseFree += 1;
        }

        private void Update()
        {
            if (Stage == RoundStage.None) return;

            if (UnityEngine.Input.GetKeyDown(KeyCode.U) &&
                (Stage == RoundStage.FatMouse || Stage == RoundStage.ThinMouse))
            {
                _player.PosessAnother();
            }
            
            Timer -= Time.deltaTime;

            if (Timer < 30f && (Stage == RoundStage.FatMouse || Stage == RoundStage.ThinMouse) && !_evacuationActivated)
            {
                var message = new ActivateEvacuationMessage();
                message.active = true;
                _messageBroker.Trigger(ref message);
                _evacuationActivated = true;
            }

            if (Timer > 0f) return;

            if (Stage == RoundStage.FatMouse)
            {
                var message = new ChangeRoundMessage();
                _messageBroker.Trigger(ref message);
                Timer = _settings.RoundLengthSeconds;
                Stage = RoundStage.ThinMouse;
                _player.PosessThinMouse();
                var evacuationMessage = new ActivateEvacuationMessage();
                evacuationMessage.active = false;
                _messageBroker.Trigger(ref message);
                _evacuationActivated = false;
                
                _loseGameReason = LoseGameReason.TimeOut;
                Timer = _settings.playerDetectedToLoseSeconds;
                Stage = RoundStage.WaitToLose;
                _player.UnposessAll();
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
            SoundService.StopSound();
            SoundService.PlayMusic(MusicType.Lose);
            Stage = RoundStage.None;
            _player.UnposessAll();

            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            var loseGameWindow = windowsSystem.CreateWindow<LoseScreen>();
            loseGameWindow.Initialize(_loseGameReason);
        }
    }
}