using Common.DI;
using Networking.Dataframes.InGame;
using Startup;
using UnityEngine;

namespace UI.WindowsSystem.WindowTypes
{
    public class IntroScreen : WindowBase
    {
        [SerializeField] private Animation _animation;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _playSeconds;
        [SerializeField] private GameObject _otherSkippedLabel;

        [Inject] private GameInitializer _gameInitializer;
        
        private bool _otherSkipped;
        private bool _skipped;
        
        private float _timer;
        
        private void Start()
        {
            _timer = _playSeconds;

            _animation.Play();
            _audioSource.Play();
            
            _otherSkippedLabel.SetActive(false);

            _gameClient.Client.Subscribe<SkipIntroDataframe>(ProcessSkipIntro);
        }

        private void OnDestroy()
        {
            _gameClient.Client.Unsubscribe<SkipIntroDataframe>(ProcessSkipIntro);
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
            _skipped = true;
            
            if (_gameClient.IsConnected)
            {
                var dataframe = new SkipIntroDataframe();
                _gameClient.Send(ref dataframe);
                
                if (!_otherSkipped)
                    return;
            }
            
            _windowsSystem.DestroyWindow(this);
            _gameInitializer.StartGame();
        }

        private void ProcessSkipIntro(SkipIntroDataframe dataframe)
        {
            _otherSkipped = true;
            _otherSkippedLabel.SetActive(true);

            if (!_skipped) return;
            
            _windowsSystem.DestroyWindow(this);
            _gameInitializer.StartGame();
        }
    }
}