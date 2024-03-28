using System;
using UnityEngine;

namespace GameCore.Sounds.Playback
{
    [Serializable]
    public class MusicContainer
    {
        [SerializeField] public AudioClip audioClip;
        [SerializeField] public MusicType musicType;
    }
}