using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Sounds
{
    public enum SoundType
    {
        ButtonClick,
        Buff,
        Death,
    }

    public class SoundService : MonoBehaviour
    {
        [SerializeField] private AudioSource soundsSource;
        [SerializeField] private AudioSource firstTrackSource;
        [SerializeField] private AudioSource secondTrackSource;

        [SerializeField] private AudioClip[] tracks;

        [SerializeField] private AudioClip clickSound;
        [SerializeField] private AudioClip buffSound;
        [SerializeField] private AudioClip deathSound;

        [SerializeField] float fadingTime = 5.0f;

        private Coroutine _fadingCoroutine;

        private Dictionary<SoundType, AudioClip> _sounds;

        private void Start()
        {
            _sounds = new Dictionary<SoundType, AudioClip>();
            _sounds.Add(SoundType.ButtonClick, clickSound);
            _sounds.Add(SoundType.Buff, buffSound);
            _sounds.Add(SoundType.Death, deathSound);
            PlayMusic();
        }

        public void PlaySound(SoundType soundType)
        {
            var currentSound = _sounds[soundType];
            if (currentSound) soundsSource.PlayOneShot(currentSound);
        }

        private AudioClip GetRandomTrack()
        {
            return tracks[Random.Range(0, tracks.Length)];
        }

        public void PlayMusic()
        {
            _fadingCoroutine = StartCoroutine(FadeTracks(GetRandomTrack()));
        }

        private void Update()
        {
            if (firstTrackSource.clip == null) return;
            if (firstTrackSource.clip.length - firstTrackSource.time <= fadingTime)
            {
                if (_fadingCoroutine != null)
                {
                    return;
                }

                _fadingCoroutine = StartCoroutine(FadeTracks(GetRandomTrack()));
            }
        }

        private IEnumerator FadeTracks(AudioClip nextTrack)
        {
            secondTrackSource.clip = nextTrack;
            firstTrackSource.volume = 1.0f;
            secondTrackSource.volume = 0.0f;
            secondTrackSource.Play();

            float time = 0.0f;
            while (time < fadingTime)
            {
                float t = time / fadingTime;

                firstTrackSource.volume = Mathf.Lerp(1.0f, 0.0f, t);
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