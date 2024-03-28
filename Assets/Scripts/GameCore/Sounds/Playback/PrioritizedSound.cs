using System;
using UnityEngine;

namespace GameCore.Sounds.Playback
{
    [Serializable]
    public class PrioritizedSound
    {
        [SerializeField] public AudioClip audioClip;
        [SerializeField] public SoundType soundType;
        [SerializeField] public int priority;
        [SerializeField] public bool disableSound;
        [SerializeField] public float minPlayDelay;

        [NonSerialized] public float lastPlayedTime;
    }
}