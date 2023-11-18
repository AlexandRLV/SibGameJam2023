using System.Collections;
using System.Text;
using Common;
using GameCore.Common;
using GameCore.Common.Messages;
using LocalMessages;
using TMPro;
using UnityEngine;

namespace UI.WindowsSystem.WindowTypes
{
    public class InGameUI : WindowBase
    {
        [SerializeField] private int _pulseSeconds;
        [SerializeField] private float _pulseSpeed;
        [SerializeField] private float _pulseIntensityMin;
        [SerializeField] private float _pulseIntensityMax;
        [SerializeField] private TextMeshProUGUI _timerLabel;

        [SerializeField] private TextMeshProUGUI _missionsText;
        [SerializeField] private float _infoPanelShowTime;
        [SerializeField] private GameObject _infoPanel;

        [SerializeField] private GameObject[] _fatMouseLayoutObjects;
        [SerializeField] private GameObject[] _thinMouseLayoutObjects;

        private bool _initialized;
        private int _seconds;
        private StringBuilder _stringBuilder;

        private float _infoPanelTimer;
        
        private RoundController _roundController;
        private LocalMessageBroker _messageBroker;

        public void SetMissionsText(string text) => _missionsText.text = text;
        
        private IEnumerator Start()
        {
            _stringBuilder = new StringBuilder();
            while (!GameContainer.InGame.CanResolve<RoundController>())
            {
                yield return null;
            }
            
            _roundController = GameContainer.InGame.Resolve<RoundController>();
            _initialized = true;

            _messageBroker = GameContainer.Common.Resolve<LocalMessageBroker>();
            _messageBroker.Subscribe<ChangeCharacterMessage>(OnCharacterChanged);
            SetLayout(false);

            _infoPanelTimer = _infoPanelShowTime;
        }

        private void Update()
        {
            if (!_initialized) return;

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
            
            CheckPause();

            if (_roundController.Stage is not (RoundStage.FatMouse or RoundStage.ThinMouse))
            {
                _timerLabel.gameObject.SetActive(false);
                return;
            }
            
            CheckPulseTimer();

            int seconds = Mathf.RoundToInt(_roundController.Timer);
            if (_seconds == seconds) return;

            _seconds = seconds;
            
            int minutes = seconds / 60;
            seconds %= 60;
            
            _stringBuilder.Clear();
            _stringBuilder.Append(minutes);
            _stringBuilder.Append(":");
            if (seconds < 10) _stringBuilder.Append("0");
            _stringBuilder.Append(seconds);

            _timerLabel.text = _stringBuilder.ToString();
        }

        private void CheckPause()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                GameContainer.Common.Resolve<WindowsSystem>().CreateWindow<GamePause>();
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