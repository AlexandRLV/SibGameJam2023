using Common.DI;
using GameCore.Camera;
using Localization;
using PlayerSettings;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WindowsSystem.WindowTypes
{
    public class SettingsScreen : WindowBase
    {
        [SerializeField] private CameraSettings _cameraSettings;
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private Slider _sensitivitySlider;
        [SerializeField] private Toggle _invertXToggle;
        [SerializeField] private Toggle _invertYToggle;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _russianLanguageButton;
        [SerializeField] private Button _englishLanguageButton;

        [Inject] private WindowsSystem _windowsSystem;
        [Inject] private LocalizationProvider _localizationProvider;
        [Inject] private GameSettingsManager _gameSettingsManager;

        private void Start()
        {
            float sensitivity = Mathf.InverseLerp(_cameraSettings.minSensitivity, _cameraSettings.maxSensitivity, _gameSettingsManager.Settings.sensitivity);
            _sensitivitySlider.value = sensitivity;

            _invertXToggle.isOn = _gameSettingsManager.Settings.invertX;
            _invertYToggle.isOn = _gameSettingsManager.Settings.invertY;

            _volumeSlider.value = _gameSettingsManager.Settings.volume;
            _volumeSlider.onValueChanged.AddListener(value =>
            {
                _gameSettingsManager.Settings.volume = value;
                AudioListener.volume = value;
            });
            
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

        private void Save()
        {
            float sensitivity = Mathf.Lerp(_cameraSettings.minSensitivity, _cameraSettings.maxSensitivity,
                _sensitivitySlider.value);
            _gameSettingsManager.Settings.sensitivity = sensitivity;

            _gameSettingsManager.Settings.invertX = _invertXToggle.isOn;
            _gameSettingsManager.Settings.invertY = _invertYToggle.isOn;

            _gameSettingsManager.Settings.volume = _volumeSlider.value;
            
            _gameSettingsManager.Apply();
            _gameSettingsManager.Save();
            _windowsSystem.DestroyWindow(this);
        }
    }
}