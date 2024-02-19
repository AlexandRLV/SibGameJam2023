using System.Text;
using Common;
using Common.DI;
using GameCore.Common.Messages;
using GameCore.Player;
using GameCore.RoundControl;
using LocalMessages;
using Networking;
using Networking.Client;
using TMPro;
using UnityEngine;

namespace UI.WindowsSystem.WindowTypes
{
    public class InGameUI : WindowBase
    {
        [Header("Timer")]
        [SerializeField] private int _pulseSeconds;
        [SerializeField] private float _pulseSpeed;
        [SerializeField] private float _pulseIntensityMin;
        [SerializeField] private float _pulseIntensityMax;
        [SerializeField] private TextMeshProUGUI _timerLabel;

        [Header("Missions")]
        [SerializeField] private TextMeshProUGUI _missionsText;
        [SerializeField] private float _infoPanelShowTime;
        [SerializeField] private GameObject _infoPanel;

        [Header("Indicators")]
        [SerializeField] private float _poisonIndicatorHideDelay;
        [SerializeField] private GameObject _poisonIndicator;
        [SerializeField] private GameObject _interactIndicator;
        
        [Header("Layouts")]
        [SerializeField] private GameObject[] _fatMouseLayoutObjects;
        [SerializeField] private GameObject[] _thinMouseLayoutObjects;
        [SerializeField] private GameObject[] _objectsToDisableInMultiplayer;
        
        [Inject] private RoundController _roundController;
        [Inject] private WindowsSystem _windowsSystem;
        [Inject] private GameClientData _gameClientData;
        [Inject] private LocalMessageBroker _messageBroker;
        
        private bool _initialized;
        private int _seconds;
        private StringBuilder _stringBuilder;

        private float _infoPanelTimer;
        private float _poisonIndicatorHideTimer;

        public void SetMissionsText(string text) => _missionsText.text = text;

        public void SetPoisonState(bool state, bool hideWithDelay = true)
        {
            if (!state && hideWithDelay)
            {
                _poisonIndicatorHideTimer = _poisonIndicatorHideDelay;
                return;
            }
            
            _poisonIndicatorHideTimer = -1f;
            _poisonIndicator.SetActive(state);
        }
        
        private void Start()
        {
            SetPoisonState(false, false);

            _stringBuilder = new StringBuilder();
            _initialized = true;

            _messageBroker.Subscribe<ChangeCharacterMessage>(OnCharacterChanged);

            _interactIndicator.SetActive(false);
            _infoPanelTimer = _infoPanelShowTime;

            if (!_gameClientData.IsConnected)
            {
                SetLayout(false);
                return;
            }

            SetLayout(!_gameClientData.IsMaster);
            foreach (var disableObject in _objectsToDisableInMultiplayer)
            {
                disableObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _messageBroker.Unsubscribe<ChangeCharacterMessage>(OnCharacterChanged);
        }

        private void Update()
        {
            if (!_initialized) return;

            if (_poisonIndicatorHideTimer > 0f)
            {
                _poisonIndicatorHideTimer -= Time.deltaTime;
                if (_poisonIndicatorHideTimer <= 0f)
                    _poisonIndicator.SetActive(false);
            }

            if (_infoPanelTimer > 0f)
            {
                _infoPanelTimer -= Time.deltaTime;
                if (_infoPanelTimer <= 0f)
                {
                    _infoPanel.SetActive(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _infoPanel.SetActive(!_infoPanel.activeSelf);
                _infoPanelTimer = 0f;
            }

            if (_roundController.Stage != RoundStage.Game)
            {
                _timerLabel.gameObject.SetActive(false);
                return;
            }
            
            CheckPause();
            CheckPulseTimer();
            UiUtils.SetTimerText(_timerLabel, _roundController.Timer, ref _seconds);
        }

        private void CheckPause()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                _windowsSystem.CreateWindow<GamePause>();
        }

        private void CheckPulseTimer()
        {
            if (_seconds > _pulseSeconds)
            {
                _timerLabel.transform.localScale = Vector3.one;
                return;
            }

            float t = Mathf.Sin(Time.time * _pulseSpeed);
            t = Mathf.InverseLerp(-1, 1, t);
            t = Mathf.Lerp(_pulseIntensityMin, _pulseIntensityMax, t);
            
            _timerLabel.transform.localScale = Vector3.one * t;
        }

        private void OnCharacterChanged(ref ChangeCharacterMessage message)
        {
            SetLayout(message.isThinMouse);

            var player = GameContainer.InGame.Resolve<IPlayer>();
            _interactIndicator.SetActive(player.CurrentMovement.MoveValues.CurrentInteractiveObject != null);
        }

        private void SetLayout(bool isThinMouse)
        {
            foreach (var layoutObject in _fatMouseLayoutObjects)
            {
                layoutObject.SetActive(!isThinMouse);
            }
            
            foreach (var layoutObject in _thinMouseLayoutObjects)
            {
                layoutObject.SetActive(isThinMouse);
            }
        }
    }
}