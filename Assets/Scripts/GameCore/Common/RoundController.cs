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
        
        [SerializeField] private RoundSettings _settings;

        private LocalMessageBroker _messageBroker;
        private GamePlayer _player;
        
        private void Start()
        {
            Timer = _settings.RoundLengthSeconds;
            Stage = RoundStage.FatMouse;
            
            _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            _messageBroker.Subscribe<PlayerDetectedMessage>(OnPlayerDetected);

            _player = GameContainer.InGame.Resolve<GamePlayer>();
            _player.PosessFatMouse();
        }

        private void OnDestroy()
        {
            _messageBroker.Unsubscribe<PlayerDetectedMessage>(OnPlayerDetected);
        }

        private void OnPlayerDetected(ref PlayerDetectedMessage value)
        {
            Timer = _settings.playerDetectedToLoseSeconds;
            Stage = RoundStage.WaitToLose;
        }

        private void Update()
        {
            if (Stage == RoundStage.None) return;
            
            Timer -= Time.deltaTime;
            if (Timer > 0f) return;

            if (Stage == RoundStage.FatMouse)
            {
                var message = new ChangeRoundMessage();
                _messageBroker.Trigger(ref message);
                Timer = _settings.RoundLengthSeconds;
                Stage = RoundStage.ThinMouse;
                _player.PosessThinMouse();
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