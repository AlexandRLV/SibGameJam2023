using GameCore.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class LoseGameWindow : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _reasonLabel;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _menuButton;

        private void Start()
        {
            _restartButton.onClick.AddListener(() =>
            {
                // TODO: restart
            });
            
            _menuButton.onClick.AddListener(() =>
            {
                // TODO: go to menu
            });
        }

        public void Initialize(LoseGameReason reason)
        {
            _reasonLabel.text = reason switch
            {
                LoseGameReason.TimeOut => "Время вышло!",
                LoseGameReason.Catched => "Тебя поймали!",
                LoseGameReason.Dead => "Ты погиб!"
            };
        }
    }
}