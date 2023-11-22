using System;
using TMPro;
using UnityEngine;

namespace UI.WindowsSystem.WindowTypes.Multiplayer.Rooms
{
    public class GameStartTimerPopup : MonoBehaviour
    {
        private const float StartTime = 3f;
        
        public Action OnTimerEnd;
        
        [SerializeField] private TextMeshProUGUI _timerText;

        private float _timer;
        private int _seconds;

        private void OnEnable()
        {
            _timer = StartTime;
            _seconds = -1;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;

            if (_timer < 0f)
            {
                OnTimerEnd?.Invoke();
                return;
            }
            
            int seconds = Mathf.CeilToInt(_timer);
            if (_seconds == seconds) return;

            _timerText.text = seconds.ToString();
        }
    }
}