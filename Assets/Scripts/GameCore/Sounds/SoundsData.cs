using Common;
using UnityEngine;

namespace GameCore.Sounds
{
    [CreateAssetMenu(fileName = "Sounds Data")]
    public class SoundsData : ScriptableObject
    {
        [SerializeField] [ArrayDisplayPropertyName("soundType")] public PrioritizedSound[] prioritizedSounds;
        [SerializeField] [ArrayDisplayPropertyName("soundType")] public SoundContainer[] sounds;
        [SerializeField] [ArrayDisplayPropertyName("musicType")] public MusicContainer[] music;
    }
}