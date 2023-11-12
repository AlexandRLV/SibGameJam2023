using Common;
using GameCore.Camera;
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

        private WindowsSystem _windowsSystem;
        
        private void Start()
        {
            _windowsSystem = GameContainer.Common.Resolve<WindowsSystem>();
            
            float sensitivity = Mathf.InverseLerp(_cameraSettings.minSensitivity, _cameraSettings.maxSensitivity,
                _cameraSettings.sensitivity);
            _sensitivitySlider.value = sensitivity;

            _invertXToggle.isOn = _cameraSettings.invertX;
            _invertYToggle.isOn = _cameraSettings.invertY;

            _volumeSlider.value = AudioListener.volume;
            
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