using System.Collections.Generic;
using GameCore.LevelObjects.FloorTypeDetection;
using UnityEngine;

namespace GameCore.Sounds.ChairMovementSound
{
    [CreateAssetMenu(menuName = "Configs/Chair Movement Sounds Data")]
    public class ChairMovementSoundsData : ScriptableObject
    {
        [SerializeField] public int defaultSoundId;
        [SerializeField] public ChairSoundContainer[] containers;

        private Dictionary<FloorType, AudioClip> _audioClipsByType;

        public void Initialize()
        {
            if (_audioClipsByType != null) return;
            
            _audioClipsByType = new Dictionary<FloorType, AudioClip>();
            foreach (var container in containers)
            {
                _audioClipsByType.Add(container.floorType, container.audioClip);
            }
        }
        
        public AudioClip GetClipForFloorType(FloorType floorType)
        {
            return _audioClipsByType.GetValueOrDefault(floorType, containers[defaultSoundId].audioClip);
        }
    }
}