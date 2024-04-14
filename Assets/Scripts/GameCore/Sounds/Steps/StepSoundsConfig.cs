using Common;
using GameCore.LevelObjects.FloorTypeDetection;
using UnityEngine;

namespace GameCore.Sounds.Steps
{
    [CreateAssetMenu(menuName = "Configs/Step Sounds")]
    public class StepSoundsConfig : ScriptableObject
    {
        [SerializeField] public int defaultTypesId;
        [SerializeField] public StepSoundsTypeContainer[] soundsTypes;

        public AudioClip GetRandomClipForFloorType(FloorType floorType)
        {
            foreach (var container in soundsTypes)
            {
                if (container.floorType == floorType)
                    return container.clips.GetRandom();
            }

            return soundsTypes[defaultTypesId].clips.GetRandom();
        }
    }
}