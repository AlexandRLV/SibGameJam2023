using Common;
using Common.DI;
using Startup;
using UnityEngine;

namespace UI.WindowsSystem.WindowTypes
{
    public class IntroScreen : WindowBase
    {
        [SerializeField] private Animation _animation;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _playSeconds;

        private float _timer;
        private GameInitializer _gameInitializer;
        
        private void Awake()
        {
            _gameInitializer = GameContainer.Common.Resolve<GameInitializer>();
            
            _timer = _playSeconds;

            _animation.Play();
            _audioSource.Play();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
                return;
            }
            
            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            
            StartGame();
        }

        private void StartGame()
        {
            var windowSystem = GameContainer.Common.Resolve<WindowsSystem>();
            windowSystem.DestroyWindow(this);
            _gameInitializer.StartGame();
        }
    }
}