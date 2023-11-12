using Common;
using UnityEngine;

namespace UI.WindowsSystem.WindowTypes
{
    public class TitlesScreen : WindowBase
    {
        [SerializeField] private float _showTime;

        private float _timer;

        private void Start()
        {
            _timer = _showTime;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
                return;
            }
            
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
         
            Close();
        }

        private void Close()
        {
            GameContainer.Common.Resolve<WindowsSystem>().DestroyWindow(this);
        }
    }
}