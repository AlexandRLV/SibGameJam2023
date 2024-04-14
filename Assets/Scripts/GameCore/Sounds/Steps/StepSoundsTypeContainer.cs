using System;
using GameCore.LevelObjects.FloorTypeDetection;
using UnityEngine;

namespace GameCore.Sounds.Steps
{
    [Serializable]
    public class StepSoundsTypeContainer
    {
        [SerializeField] public FloorType floorType;
        [SerializeField] public AudioClip[] clips;
    }
}