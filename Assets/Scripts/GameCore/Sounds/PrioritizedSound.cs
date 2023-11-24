using System;
using UnityEngine;

namespace GameCore.Sounds
{
    [Serializable]
    public class PrioritizedSound
    {
        [SerializeField] public AudioClip audioClip;
        [SerializeField] public SoundType soundType;
        [SerializeField] public int priority;
    }
}