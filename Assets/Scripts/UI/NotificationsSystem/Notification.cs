using TMPro;
using UnityEngine;

namespace UI.NotificationsSystem
{
    public class Notification : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void Initialize(string value)
        {
            _text.text = value;
        }
    }
}