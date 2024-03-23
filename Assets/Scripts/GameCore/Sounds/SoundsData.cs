using Common;
using GameCore.Sounds.Playback;
using UnityEngine;

namespace GameCore.Sounds
{
    [CreateAssetMenu(menuName = "Configs/Sounds Data")]
    public class SoundsData : ScriptableObject
    {
        [SerializeField] [ArrayDisplayPropertyName("soundType")] public PrioritizedSound[] prioritizedSounds;
        [SerializeField] [ArrayDisplayPropertyName("soundType")] public SoundContainer[] sounds;
        [SerializeField] [ArrayDisplayPropertyName("musicType")] public MusicContainer[] music;
    }
}