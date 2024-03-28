using System.Collections;
using System.Collections.Generic;
using Common.DI;
using GameCore.Sounds.Playback;
using GameCore.Sounds.Volume;
using PlayerSettings;
using UnityEngine;

namespace GameCore.Sounds
{
    public class SoundService : MonoBehaviour
    {
        [SerializeField] private AudioSource _soundsSource;
        [SerializeField] private AudioSource _voiceSource;
        [SerializeField] private AudioSource _firstTrackSource;
        [SerializeField] private AudioSource _secondTrackSource;

        [SerializeField] private float _fadingTime;
        [SerializeField] private SoundsData _soundsData;
        
        [Inject] private GameSettingsManager _gameSettingsManager;
        
        private Coroutine _fadingCoroutine;

        private Dictionary<SoundType, PrioritizedSound> _prioritizedSounds;
        
        private Dictionary<SoundType, SoundContainer> _sounds;
        private Dictionary<MusicType, AudioClip> _tracks;

        private int _playingSoundPriority;
        private float _musicVolume;
        
        private void Awake()
        {
            GameContainer.InjectToInstance(this);
            
            _sounds = new Dictionary<SoundType, SoundContainer>();
            _tracks = new Dictionary<MusicType, AudioClip>();

            _prioritizedSounds = new Dictionary<SoundType, PrioritizedSound>();
            foreach (var sound in _soundsData.prioritizedSounds)
            {
                if (!_prioritizedSounds.TryAdd(sound.soundType, sound))
                    Debug.LogError($"Ошибка: повторяющийся тип приоритетного звука: {sound.soundType}");
            }

            foreach (var sound in _soundsData.sounds)
            {
                if (!_sounds.TryAdd(sound.soundType, sound))
                    Debug.LogError($"Ошибка: повторяющийся тип звука: {sound.soundType}");
            }

            foreach (var music in _soundsData.music)
            {
                if (!_tracks.TryAdd(music.musicType, music.audioClip))
                    Debug.LogError($"Ошибка: повторяющийся тип музыки: {music.musicType}");
            }
            
            _gameSettingsManager.RegisterVolumeListener(SoundVolumeType.Music, UpdateMusicVolume);
            _musicVolume = _gameSettingsManager.GetVolume(SoundVolumeType.Music);
            
            _firstTrackSource.volume = _musicVolume;
            _secondTrackSource.volume = 0f;
        }

        public void PlaySound(SoundType soundType)
        {
            if (_prioritizedSounds.TryGetValue(soundType, out var sound))
            {
                if (sound.disableSound) return;
                if (Time.time - sound.lastPlayedTime < sound.minPlayDelay) return;
                
                PlaySoundByPriority(sound);
            }
            else
            {
                var container = _sounds[soundType];
                if (container != null && !container.disableSound)
                    _soundsSource.PlayOneShot(container.audioClip);
            }
        }

        private void PlaySoundByPriority(PrioritizedSound sound)
        {
            if (_voiceSource.isPlaying && sound.priority <= _playingSoundPriority)
                return;

            sound.lastPlayedTime = Time.time;
            _playingSoundPriority = sound.priority;
            _voiceSource.clip = sound.audioClip;
            _voiceSource.Play();
        }

        public void PlayMusic(MusicType musicType, bool adjustTime = false)
        {
            var clip = _tracks[musicType];
            if (clip == null)
            {
                Debug.LogError($"У музыки {musicType} нет аудиоклипа! Прокиньте нужный");
                StopMusic();
                return;
            }
            
            if (_firstTrackSource.isPlaying)
            {
                FadeToMusic(clip, adjustTime);
            }
            else
            {
                _firstTrackSource.clip = clip;
                _firstTrackSource.Play();
            }
        }

        public void StopSound()
        {
            _soundsSource.Stop();
        }
        
        public void StopMusic()
        {
            _firstTrackSource.Stop();
            _secondTrackSource.Stop();
        }

        public void PlayRandomSound(params SoundType[] sounds)
        {
            int randomNumber = Random.Range(0, sounds.Length);
            PlaySound(sounds[randomNumber]);
        }

        private void UpdateMusicVolume(float volume)
        {
            _musicVolume = volume;
            if (_fadingCoroutine != null) return;

            _firstTrackSource.volume = volume;
        }

        private void FadeToMusic(AudioClip clip, bool adjustTime = false)
        {
            if (_fadingCoroutine != null)
                StopCoroutine(_fadingCoroutine);

            _fadingCoroutine = StartCoroutine(FadeTracks(clip, adjustTime));
        }

        private IEnumerator FadeTracks(AudioClip nextTrack, bool adjustTime = false)
        {
            _secondTrackSource.clip = nextTrack;
            _secondTrackSource.volume = 0.0f;
            _secondTrackSource.Play();
            if (adjustTime)
                _secondTrackSource.time = _firstTrackSource.time;

            float firstVolume = _firstTrackSource.volume;
            float volumePercent = firstVolume / _musicVolume;

            float time = 0.0f;
            while (time < _fadingTime)
            {
                firstVolume = _musicVolume * volumePercent;
                float t = time / _fadingTime;

                _firstTrackSource.volume = Mathf.Lerp(firstVolume, 0.0f, t);
                _secondTrackSource.volume = Mathf.Lerp(0.0f, firstVolume, t);

                time += Time.deltaTime;
                yield return null;
            }

            _firstTrackSource.volume = 0.0f;
            _secondTrackSource.volume = _musicVolume;
            _firstTrackSource.Stop();
            (_firstTrackSource, _secondTrackSource) = (_secondTrackSource, _firstTrackSource);
            _fadingCoroutine = null;
        }
    }
}