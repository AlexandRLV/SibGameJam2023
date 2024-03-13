using Common.DI;
using GameCore.Camera;
using Localization;
using Newtonsoft.Json;
using UnityEngine;

namespace PlayerSettings
{
    public class GameSettingsManager
    {
        private const string PrefsKey = "GameSettings";
        
        public PlayerSettings Settings { get; private set; }

        [Inject] private LocalizationProvider _localizationProvider;
        [Inject] private CameraSettings _cameraSettings;

        public void Initialize()
        {
            if (!PlayerPrefs.HasKey(PrefsKey))
            {
                CreateDefault();
                return;
            }
            
            try
            {
                string json = PlayerPrefs.GetString(PrefsKey);
                Settings = JsonConvert.DeserializeObject<PlayerSettings>(json);
            }
            catch
            {
                CreateDefault();
            }
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(Settings);
            PlayerPrefs.SetString(PrefsKey, json);
        }

        public void Apply()
        {
            if (Settings == null)
                return;

            AudioListener.volume = Settings.volume;
            _localizationProvider.SetLanguage(Settings.Language);
            _cameraSettings.sensitivity = Settings.sensitivity;
            _cameraSettings.invertX = Settings.invertX;
            _cameraSettings.invertY = Settings.invertY;
        }

        private void CreateDefault()
        {
            Settings = new PlayerSettings
            {
                volume = 1f,
                sensitivity = _cameraSettings.sensitivity,
                invertX = _cameraSettings.invertX,
                invertY = _cameraSettings.invertY,
                Language = _localizationProvider.CurrentLanguage
            };
        }
    }
}