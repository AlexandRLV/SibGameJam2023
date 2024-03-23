using Common.DI;
using GameCore.Camera;
using GameCore.Sounds.Volume;
using Localization;
using PlayerSettings;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class SettingsScreen : WindowBase
    {
        [Header("Volume settings")]
        [SerializeField] private Button _musicToggleButton;
        [SerializeField] private Button _soundsToggleButton;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _soundsVolumeSlider;
        [SerializeField] private GameObject _musicOnState;
        [SerializeField] private GameObject _musicOffState;
        [SerializeField] private GameObject _soundsOnState;
        [SerializeField] private GameObject _soundsOffState;
        
        [Header("Camera settings")]
        [SerializeField] private CameraSettings _cameraSettings;
        [SerializeField] private Slider _sensitivitySlider;
        [SerializeField] private Toggle _invertXToggle;
        [SerializeField] private Toggle _invertYToggle;
        
        [Header("Language settings")]
        [SerializeField] private Button _russianLanguageButton;
        [SerializeField] private Button _englishLanguageButton;
        
        [Header("Control")]
        [SerializeField] private Button _saveButton;

        [Inject] private WindowsSystem _windowsSystem;
        [Inject] private LocalizationProvider _localizationProvider;
        [Inject] private GameSettingsManager _gameSettingsManager;

        private void Start()
        {
            float sensitivity = Mathf.InverseLerp(_cameraSettings.minSensitivity, _cameraSettings.maxSensitivity, _gameSettingsManager.Settings.sensitivity);
            _sensitivitySlider.value = sensitivity;

            _invertXToggle.isOn = _gameSettingsManager.Settings.invertX;
            _invertYToggle.isOn = _gameSettingsManager.Settings.invertY;

            var musicSettings = _gameSettingsManager.GetSoundSettings(SoundVolumeType.Music);
            var soundSettings = _gameSettingsManager.GetSoundSettings(SoundVolumeType.Sound);

            _musicOnState.SetActive(musicSettings.enabled);
            _musicOffState.SetActive(!musicSettings.enabled);
            
            _soundsOnState.SetActive(soundSettings.enabled);
            _soundsOffState.SetActive(!soundSettings.enabled);

            _musicVolumeSlider.value = musicSettings.Volume;
            _soundsVolumeSlider.value = soundSettings.Volume;
            
            _musicVolumeSlider.onValueChanged.AddListener(MusicVolumeChanged);
            _soundsVolumeSlider.onValueChanged.AddListener(SoundsVolumeChanged);
            
            _musicToggleButton.onClick.AddListener(ToggleMusic);
            _soundsToggleButton.onClick.AddListener(ToggleSounds);
            
            _russianLanguageButton.onClick.AddListener(() =>
            {
                _gameSettingsManager.Settings.Language = SystemLanguage.Russian;
                _localizationProvider.SetLanguage(SystemLanguage.Russian);
            });
            _englishLanguageButton.onClick.AddListener(() =>
            {
                _gameSettingsManager.Settings.Language = SystemLanguage.English;
                _localizationProvider.SetLanguage(SystemLanguage.English);
            });
            
            _saveButton.onClick.AddListener(Save);
        }

        private void MusicVolumeChanged(float volume)
        {
            _gameSettingsManager.SetVolume(SoundVolumeType.Music, volume);
            
            _musicOnState.SetActive(true);
            _musicOffState.SetActive(false);
        }

        private void SoundsVolumeChanged(float volume)
        {
            _gameSettingsManager.SetVolume(SoundVolumeType.Sound, volume);
            
            _soundsOnState.SetActive(true);
            _soundsOffState.SetActive(false);
        }

        private void ToggleMusic()
        {
            var settings = _gameSettingsManager.GetSoundSettings(SoundVolumeType.Music);
            bool musicEnabled = settings.enabled;
            musicEnabled = !musicEnabled;
            
            _gameSettingsManager.SetVolume(SoundVolumeType.Music, 0f);
            _gameSettingsManager.SetEnabled(SoundVolumeType.Music, musicEnabled);
            
            _musicOnState.SetActive(musicEnabled);
            _musicOffState.SetActive(!musicEnabled);
            _musicVolumeSlider.SetValueWithoutNotify(0f);
        }

        private void ToggleSounds()
        {
            var settings = _gameSettingsManager.GetSoundSettings(SoundVolumeType.Sound);
            bool soundEnabled = settings.enabled;
            soundEnabled = !soundEnabled;
            
            _gameSettingsManager.SetVolume(SoundVolumeType.Sound, 0f);
            _gameSettingsManager.SetEnabled(SoundVolumeType.Sound, soundEnabled);
            
            _soundsOnState.SetActive(soundEnabled);
            _soundsOffState.SetActive(!soundEnabled);
            _soundsVolumeSlider.SetValueWithoutNotify(0f);
        }

        private void Save()
        {
            float sensitivity = Mathf.Lerp(_cameraSettings.minSensitivity, _cameraSettings.maxSensitivity,
                _sensitivitySlider.value);
            _gameSettingsManager.Settings.sensitivity = sensitivity;

            _gameSettingsManager.Settings.invertX = _invertXToggle.isOn;
            _gameSettingsManager.Settings.invertY = _invertYToggle.isOn;
            
            _gameSettingsManager.SetVolume(SoundVolumeType.Music, _musicVolumeSlider.value);
            _gameSettingsManager.SetVolume(SoundVolumeType.Sound, _soundsVolumeSlider.value);
            
            _gameSettingsManager.Apply();
            _gameSettingsManager.Save();
            _windowsSystem.DestroyWindow(this);
        }
    }
}