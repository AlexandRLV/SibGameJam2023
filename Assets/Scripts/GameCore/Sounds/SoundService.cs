using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Sounds
{
    public enum SoundType
    {
        Buff,
        Alert,
        AboutFat,
        ThinCats1,
        ThinCats2,
        ThinCheese,
        ThinDetect,
        ThinPanel,
        ThinCactus,
        ThinHostage,
        FatCats1,
        FatCats2,
        FatCats3,
        FatCheese,
        FatDetect,
        FatPanel,
        FatHostage,
        Eating,
        Mousetrap1,
        Mousetrap2,
        Mousetrap3,
        Panel,
        SubmissionComplete,
        
        // UI Sounds
        Click,
        Hover1,
        Hover2,
        Hover3,
    }

    public enum MusicType
    {
        Menu,
        ThinCharacter,
        FatCharacter,
        Lose,
        Win
    }

    public class SoundService : MonoBehaviour
    {
        [SerializeField] private AudioSource soundsSource;
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
        [SerializeField] private AudioClip hover1Sound;
        [SerializeField] private AudioClip hover2Sound;
        [SerializeField] private AudioClip hover3Sound;

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

        [SerializeField] float fadingTime = 1.0f;
        
        private Coroutine _fadingCoroutine;

        private Dictionary<SoundType, AudioClip> _sounds;
        private Dictionary<MusicType, AudioClip> _tracks;

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
            _sounds.Add(SoundType.ThinCats1, thinCats1);
            _sounds.Add(SoundType.ThinCats2, thinCats2);
            _sounds.Add(SoundType.ThinCheese, thinCheese);
            _sounds.Add(SoundType.ThinDetect, thinDetect);
            _sounds.Add(SoundType.ThinPanel, thinPanel);
            _sounds.Add(SoundType.ThinCactus, thinCactus);
            _sounds.Add(SoundType.ThinHostage, thinHostage);
            _sounds.Add(SoundType.FatCats1, fatCats1);
            _sounds.Add(SoundType.FatCats2, fatCats2);
            _sounds.Add(SoundType.FatCats3, fatCats3);
            _sounds.Add(SoundType.FatCheese, fatCheese);
            _sounds.Add(SoundType.FatDetect, fatDetect);
            _sounds.Add(SoundType.FatPanel, fatPanel);
            _sounds.Add(SoundType.FatHostage, fatHostage);
            _sounds.Add(SoundType.Alert, alertSound);
            _sounds.Add(SoundType.SubmissionComplete, submissionCompleteSound);
            
            // UI
            _sounds.Add(SoundType.Click, clickSound);
            _sounds.Add(SoundType.Hover1, hover1Sound);
            _sounds.Add(SoundType.Hover2, hover2Sound);
            _sounds.Add(SoundType.Hover3, hover3Sound);

            _tracks.Add(MusicType.Menu, menuTrack);
            _tracks.Add(MusicType.ThinCharacter, thinTrack);
            _tracks.Add(MusicType.FatCharacter, fatTrack);
            _tracks.Add(MusicType.Lose, loseTrack);
            _tracks.Add(MusicType.Win, winTrack);
        }

        public void PlaySound(SoundType soundType)
        {
            var currentSound = _sounds[soundType];
            if (currentSound) soundsSource.PlayOneShot(currentSound);
        }

        public void PlayMusic(MusicType musicType)
        {
            var clip = _tracks[musicType];
            if (firstTrackSource.isPlaying)
            {
                FadeToMusic(clip);
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

        private void FadeToMusic(AudioClip clip)
        {
            if (_fadingCoroutine != null)
                StopCoroutine(_fadingCoroutine);

            _fadingCoroutine = StartCoroutine(FadeTracks(clip));
        }
        
        private IEnumerator FadeTracks(AudioClip nextTrack)
        {
            secondTrackSource.clip = nextTrack;
            secondTrackSource.volume = 0.0f;
            secondTrackSource.Play();

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