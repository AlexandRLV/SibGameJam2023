using Common;
using GameCore.Common.Messages;
using GameCore.Player;
using LocalMessages;
using UI.WindowsSystem;
using UI.WindowsSystem.WindowTypes;
using UnityEngine;

namespace GameCore.Common
{
    public class RoundController : MonoBehaviour
    {
        public float Timer { get; private set; }
        public RoundStage Stage { get; private set; }
        public RoundData Data { get; private set; }

        [SerializeField] private RoundSettings _settings;

        private LocalMessageBroker _messageBroker;
        private GamePlayer _player;
        bool evacuationActivated = false;

        private void Start()
        {
            Data = new RoundData();
            Timer = _settings.RoundLengthSeconds;
            Stage = RoundStage.FatMouse;

            _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            _messageBroker.Subscribe<PlayerDetectedMessage>(OnPlayerDetected);
            _messageBroker.Subscribe<PlayerWinMessage>(OnPlayerWin);

            _player = GameContainer.InGame.Resolve<GamePlayer>();
            _player.PosessFatMouse();
        }

        private void OnPlayerWin(ref PlayerWinMessage value)
        {
            Stage = RoundStage.None;
            _player.UnposessAll();

            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowsSystem.CreateWindow<WinScreen>();
        }

        private void OnDestroy()
        {
            _messageBroker.Unsubscribe<PlayerDetectedMessage>(OnPlayerDetected);
            _messageBroker.Unsubscribe<PlayerWinMessage>(OnPlayerWin);
        }

        private void OnPlayerDetected(ref PlayerDetectedMessage value)
        {
            Timer = _settings.playerDetectedToLoseSeconds;
            Stage = RoundStage.WaitToLose;
            _player.UnposessAll();
        }

        public void CatchCactus()
        {
            Data.CactusCatched = true;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.T))
            {
                Stage = RoundStage.None;
                _player.UnposessAll();

                var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
                windowsSystem.CreateWindow<WinScreen>();
                return;
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.H))
            {
                LoseGame(LoseGameReason.Catched);
                return;
            }

            if (Stage == RoundStage.None) return;

            if (UnityEngine.Input.GetKeyDown(KeyCode.U))
                _player.PosessAnother();

            Timer -= Time.deltaTime;

            if (Timer < 30f && (Stage == RoundStage.FatMouse || Stage == RoundStage.ThinMouse) && !evacuationActivated)
            {
                var message = new ActivateEvacuationMessage();
                message.active = true;
                _messageBroker.Trigger(ref message);
                evacuationActivated = true;
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
                evacuationActivated = false;

            }
            else if (Stage == RoundStage.ThinMouse)
            {
                LoseGame(LoseGameReason.TimeOut);
            }
            else if (Stage == RoundStage.WaitToLose)
            {
                LoseGame(LoseGameReason.Catched);
            }
        }

        private void LoseGame(LoseGameReason reason)
        {
            Stage = RoundStage.None;
            _player.UnposessAll();

            var windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            var loseGameWindow = windowsSystem.CreateWindow<LoseScreen>();
            loseGameWindow.Initialize(reason);
        }
    }
}