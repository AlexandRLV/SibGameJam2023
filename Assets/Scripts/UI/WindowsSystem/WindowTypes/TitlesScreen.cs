using Common.DI;
using UnityEngine;

namespace UI.WindowsSystem.WindowTypes
{
    public class TitlesScreen : WindowBase
    {
        [SerializeField] private float _showTime;
        
        [Inject] private WindowsSystem _windowsSystem;

        private float _timer;

        private void Start()
        {
            _timer = _showTime;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
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
            _windowsSystem.DestroyWindow(this);
        }
    }
}