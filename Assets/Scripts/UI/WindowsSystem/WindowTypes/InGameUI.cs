using System.Collections;
using System.Text;
using Common;
using GameCore.Common;
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

        private bool _initialized;
        private int _seconds;
        private StringBuilder _stringBuilder;
        private RoundController _roundController;
        
        private IEnumerator Start()
        {
            _stringBuilder = new StringBuilder();
            while (!GameContainer.InGame.CanResolve<RoundController>())
            {
                yield return null;
            }
            
            _roundController = GameContainer.InGame.Resolve<RoundController>();
            _initialized = true;
        }

        private void Update()
        {
            if (!_initialized) return;
            
            CheckPause();
            CheckPulseTimer();

            int seconds = Mathf.RoundToInt(_roundController.Timer);
            if (_seconds == seconds) return;

            _seconds = seconds;
            
            int minutes = seconds / 60;
            seconds %= 60;
            
            _stringBuilder.Clear();
            _stringBuilder.Append("Time left: ");
            _stringBuilder.Append(minutes);
            _stringBuilder.Append(":");
            if (seconds < 10) _stringBuilder.Append("0");
            _stringBuilder.Append(seconds);

            _timerLabel.text = _stringBuilder.ToString();
        }

        private void CheckPause()
        {
            if (Input.GetKeyDown(KeyCode.P))
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
    }
}