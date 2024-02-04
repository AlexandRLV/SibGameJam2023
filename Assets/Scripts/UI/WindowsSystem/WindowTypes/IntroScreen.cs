using Common.DI;
using LocalMessages;
using Networking;
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
        [Inject] private WindowsSystem _windowsSystem;
        [Inject] private GameClientData _gameClientData;
        [Inject] private IGameClient _gameClient;
        [Inject] private LocalMessageBroker _messageBroker;
        
        private bool _otherSkipped;
        private bool _skipped;
        
        private float _timer;
        
        private void Start()
        {
            _timer = _playSeconds;

            _animation.Play();
            _audioSource.Play();
            
            _otherSkippedLabel.SetActive(false);

            _messageBroker.Subscribe<SkipIntroDataframe>(ProcessSkipIntro);
        }

        private void OnDestroy()
        {
            _messageBroker.Unsubscribe<SkipIntroDataframe>(ProcessSkipIntro);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
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
            
            if (_gameClientData.IsConnected)
            {
                var dataframe = new SkipIntroDataframe();
                _gameClient.Send(ref dataframe);
                
                if (!_otherSkipped)
                    return;
            }
            
            _windowsSystem.DestroyWindow(this);
            _gameInitializer.StartGame(false);
        }

        private void ProcessSkipIntro(ref SkipIntroDataframe dataframe)
        {
            _otherSkipped = true;
            _otherSkippedLabel.SetActive(true);

            if (!_skipped) return;
            
            _windowsSystem.DestroyWindow(this);
            _gameInitializer.StartGame(false);
        }
    }
}