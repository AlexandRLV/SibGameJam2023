using Common.DI;
using PlayerSettings;
using UnityEngine;

namespace GameCore.Sounds.Volume
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class SoundVolumeUpdater : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private SoundVolumeType _volumeType;

        [Inject] private GameSettingsManager _gameSettingsManager;

        private float _defaultVolume;

        private void Awake()
        {
            _defaultVolume = _audioSource.volume;
        }

        private void OnEnable()
        {
            GameContainer.InjectToInstance(this);
            _gameSettingsManager.RegisterVolumeListener(_volumeType, Changed);
            _audioSource.volume = _gameSettingsManager.GetVolume(_volumeType);
        }

        private void OnDisable()
        {
            _gameSettingsManager.UnregisterVolumeListener(_volumeType, Changed);
        }

        private void Changed(float volume) => _audioSource.volume = _defaultVolume * volume;
    }
}