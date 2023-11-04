using TMPro;
using UnityEngine;

namespace UI.NotificationsSystem
{
    public class Notification : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        private float _timer;

        public void Initialize(string value, float showTime)
        {
            _text.text = value;
            _timer = showTime;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            
            gameObject.SetActive(false);
        }
    }
}