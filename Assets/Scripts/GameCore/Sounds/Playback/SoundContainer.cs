using System;
using UnityEngine;

namespace GameCore.Sounds.Playback
{
    [Serializable]
    public class SoundContainer
    {
        [SerializeField] public SoundType soundType;
        [SerializeField] public AudioClip audioClip;
    }
}