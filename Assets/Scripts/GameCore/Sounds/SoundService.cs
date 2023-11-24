using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Sounds
{
    public class SoundService : MonoBehaviour
    {
        [SerializeField] private AudioSource soundsSource;
        [SerializeField] private AudioSource voiceSource;
        [SerializeField] private AudioSource firstTrackSource;
        [SerializeField] private AudioSource secondTrackSource;

        [SerializeField] private AudioClip menuTrack;
        [SerializeField] private AudioClip thinTrack;
        [SerializeField] private AudioClip fatTrack;
        [SerializeField] private AudioClip loseTrack;
        [SerializeField] private AudioClip winTrack;

        [SerializeField] private AudioClip buffSound;
        [SerializeField] private AudioClip alertSound;
        [SerializeField] private AudioClip eatingSound;
        [SerializeField] private AudioClip mousetrapSound1;
        [SerializeField] private AudioClip mousetrapSound2;
        [SerializeField] private AudioClip mousetrapSound3;
        [SerializeField] private AudioClip panelSound;
        [SerializeField] private AudioClip submissionCompleteSound;
        
        [Header("Ui")]
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private AudioClip hoverSound;

        [Header("Thin Character")]
        [SerializeField] private AudioClip thinAboutFat;

        [SerializeField] private AudioClip thinCats1;
        [SerializeField] private AudioClip thinCats2;
        [SerializeField] private AudioClip thinCheese;
        [SerializeField] private AudioClip thinDetect;
        [SerializeField] private AudioClip thinPanel;
        [SerializeField] private AudioClip thinCactus;
        [SerializeField] private AudioClip thinHostage;

        [Header("Fat Character")]
        [SerializeField] private AudioClip fatCats1;

        [SerializeField] private AudioClip fatCats2;
        [SerializeField] private AudioClip fatCats3;
        [SerializeField] private AudioClip fatCheese;
        [SerializeField] private AudioClip fatDetect;
        [SerializeField] private AudioClip fatPanel;
        [SerializeField] private AudioClip fatHostage;

        [SerializeField] private float fadingTime = 1.0f;
        [SerializeField] private SoundsData _soundsData;
        
        private Coroutine _fadingCoroutine;

        private Dictionary<SoundType, PrioritizedSound> _prioritizedSounds;
        
        private Dictionary<SoundType, AudioClip> _sounds;
        private Dictionary<MusicType, AudioClip> _tracks;

        private int _playingSoundPriority;
        
        private void Start()
        {
            _sounds = new Dictionary<SoundType, AudioClip>();
            _tracks = new Dictionary<MusicType, AudioClip>();
            _sounds.Add(SoundType.Buff, buffSound);
            _sounds.Add(SoundType.Eating, eatingSound);
            _sounds.Add(SoundType.Mousetrap1, mousetrapSound1);
            _sounds.Add(SoundType.Mousetrap2, mousetrapSound2);
            _sounds.Add(SoundType.Mousetrap3, mousetrapSound3);
            _sounds.Add(SoundType.Panel, panelSound);
            _sounds.Add(SoundType.Alert, alertSound);
            _sounds.Add(SoundType.SubmissionComplete, submissionCompleteSound);
            
            // UI
            _sounds.Add(SoundType.Click, clickSound);
            _sounds.Add(SoundType.Hover, hoverSound);

            _tracks.Add(MusicType.Menu, menuTrack);
            _tracks.Add(MusicType.ThinCharacter, thinTrack);
            _tracks.Add(MusicType.FatCharacter, fatTrack);
            _tracks.Add(MusicType.Lose, loseTrack);
            _tracks.Add(MusicType.Win, winTrack);

            _prioritizedSounds = new Dictionary<SoundType, PrioritizedSound>();
            foreach (var sound in _soundsData.prioritizedSounds)
            {
                if (_prioritizedSounds.ContainsKey(sound.soundType))
                {
                    Debug.LogError($"Ошибка: повторяющийся тип звука: {sound.soundType}");
                    continue;
                }
                
                _prioritizedSounds.Add(sound.soundType, sound);
            }
        }

        public void PlaySound(SoundType soundType)
        {
            if (_prioritizedSounds.TryGetValue(soundType, out var sound))
            {
                int priority = sound.priority;
                var clip = sound.audioClip;
                PlaySoundByPriority(clip, priority);
            }
            else
            {
                var clip = _sounds[soundType];
                if (clip != null)
                    soundsSource.PlayOneShot(clip);
            }
        }

        private void PlaySoundByPriority(AudioClip clip, int priority)
        {
            if (voiceSource.isPlaying && priority <= _playingSoundPriority)
                return;

            _playingSoundPriority = priority;
            voiceSource.clip = clip;
            voiceSource.Play();
        }

        public void PlayMusic(MusicType musicType, bool adjustTime = false)
        {
            var clip = _tracks[musicType];
            if (firstTrackSource.isPlaying)
            {
                FadeToMusic(clip, adjustTime);
            }
            else
            {
                firstTrackSource.clip = clip;
                firstTrackSource.Play();
            }
        }

        public void StopSound()
        {
            soundsSource.Stop();
        }
        
        public void StopMusic()
        {
            firstTrackSource.Stop();
            secondTrackSource.Stop();
        }

        public void PlayRandomSound(params SoundType[] sounds)
        {
            int randomNumber = Random.Range(0, sounds.Length);
            PlaySound(sounds[randomNumber]);
        }

        private void FadeToMusic(AudioClip clip, bool adjustTime = false)
        {
            if (_fadingCoroutine != null)
                StopCoroutine(_fadingCoroutine);

            _fadingCoroutine = StartCoroutine(FadeTracks(clip, adjustTime));
        }

        private IEnumerator FadeTracks(AudioClip nextTrack, bool adjustTime = false)
        {
            secondTrackSource.clip = nextTrack;
            secondTrackSource.volume = 0.0f;
            secondTrackSource.Play();
            if (adjustTime)
                secondTrackSource.time = firstTrackSource.time;

            float firstVolume = firstTrackSource.volume;

            float time = 0.0f;
            while (time < fadingTime)
            {
                float t = time / fadingTime;

                firstTrackSource.volume = Mathf.Lerp(firstVolume, 0.0f, t);
                secondTrackSource.volume = Mathf.Lerp(0.0f, 1.0f, t);

                time += Time.deltaTime;

                yield return null;
            }

            firstTrackSource.volume = 0.0f;
            secondTrackSource.volume = 1.0f;
            firstTrackSource.Stop();
            (firstTrackSource, secondTrackSource) = (secondTrackSource, firstTrackSource);
            _fadingCoroutine = null;
        }
    }
}