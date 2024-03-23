using System;
using System.Collections.Generic;
using Common.DI;
using GameCore.Camera;
using GameCore.Sounds.Volume;
using Localization;
using Newtonsoft.Json;
using UnityEngine;

namespace PlayerSettings
{
    public class GameSettingsManager
    {
        private const string PrefsKey = "GameSettings";
        private static readonly SoundVolumeType[] _volumeTypes = new[] { SoundVolumeType.Sound, SoundVolumeType.Music };
        
        public PlayerSettings Settings { get; private set; }

        [Inject] private LocalizationProvider _localizationProvider;
        [Inject] private CameraSettings _cameraSettings;

        private Dictionary<SoundVolumeType, Action<float>> _soundVolumeListeners;
        
        public void Initialize()
        {
            _soundVolumeListeners = new Dictionary<SoundVolumeType, Action<float>>();
            foreach (var volumeType in _volumeTypes)
            {
                _soundVolumeListeners.Add(volumeType, null);
            }
            
            if (!PlayerPrefs.HasKey(PrefsKey))
            {
                CreateDefault();
                return;
            }
            
            try
            {
                string json = PlayerPrefs.GetString(PrefsKey);
                Settings = JsonConvert.DeserializeObject<PlayerSettings>(json);
                if (Settings.soundSettings == null || Settings.soundSettings.Length != _volumeTypes.Length)
                    CreateDefaultSoundSettings();
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

            _localizationProvider.SetLanguage(Settings.Language);
            _cameraSettings.sensitivity = Settings.sensitivity;
            _cameraSettings.invertX = Settings.invertX;
            _cameraSettings.invertY = Settings.invertY;
            
            if (Settings.soundSettings == null) return;
            foreach (var soundSettings in Settings.soundSettings)
            {
                _soundVolumeListeners[soundSettings.soundType]?.Invoke(soundSettings.volume);
            }
        }

        private void CreateDefault()
        {
            Settings = new PlayerSettings
            {
                sensitivity = _cameraSettings.sensitivity,
                invertX = _cameraSettings.invertX,
                invertY = _cameraSettings.invertY,
                Language = _localizationProvider.CurrentLanguage,
            };
            CreateDefaultSoundSettings();
        }

        private void CreateDefaultSoundSettings()
        {
            Settings.soundSettings = new SoundSettings[]
            {
                new()
                {
                    soundType = SoundVolumeType.Music,
                    enabled = true,
                    volume = 1f
                },
                new()
                {
                    soundType = SoundVolumeType.Sound,
                    enabled = true,
                    volume = 1f
                },
            };
        }

        public float GetVolume(SoundVolumeType type) => GetSoundSettings(type).Volume;

        public SoundSettings GetSoundSettings(SoundVolumeType type)
        {
            foreach (var soundSetting in Settings.soundSettings)
            {
                if (soundSetting.soundType == type) return soundSetting;
            }

            return null;
        }

        public void SetVolume(SoundVolumeType type, float volume)
        {
            var settings = GetSoundSettings(type);
            settings.volume = volume;
            settings.enabled = true;
            
            _soundVolumeListeners[type]?.Invoke(settings.Volume);
        }

        public void SetEnabled(SoundVolumeType type, bool enabled)
        {
            var settings = GetSoundSettings(type);
            settings.enabled = enabled;
            
            _soundVolumeListeners[type]?.Invoke(settings.Volume);
        }

        public void RegisterVolumeListener(SoundVolumeType type, Action<float> callback) =>
            _soundVolumeListeners[type] += callback;
        public void UnregisterVolumeListener(SoundVolumeType type, Action<float> callback) =>
            _soundVolumeListeners[type] -= callback;
    }
}