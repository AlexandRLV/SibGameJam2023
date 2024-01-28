using Common.DI;
using GameCore.Camera;
using Localization;
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

        private void Start()
        {
            float sensitivity = Mathf.InverseLerp(_cameraSettings.minSensitivity, _cameraSettings.maxSensitivity,
                _cameraSettings.sensitivity);
            _sensitivitySlider.value = sensitivity;

            _invertXToggle.isOn = _cameraSettings.invertX;
            _invertYToggle.isOn = _cameraSettings.invertY;

            _volumeSlider.value = AudioListener.volume;
            _volumeSlider.onValueChanged.AddListener(value => AudioListener.volume = value);
            
            _russianLanguageButton.onClick.AddListener(() => _localizationProvider.SetLanguage(SystemLanguage.Russian));
            _englishLanguageButton.onClick.AddListener(() => _localizationProvider.SetLanguage(SystemLanguage.English));
            
            _saveButton.onClick.AddListener(Save);
        }

        private void Save()
        {
            float sensitivity = Mathf.Lerp(_cameraSettings.minSensitivity, _cameraSettings.maxSensitivity,
                _sensitivitySlider.value);
            _cameraSettings.sensitivity = sensitivity;

            _cameraSettings.invertX = _invertXToggle.isOn;
            _cameraSettings.invertY = _invertYToggle.isOn;

            AudioListener.volume = _volumeSlider.value;
            
            _windowsSystem.DestroyWindow(this);
        }
    }
}