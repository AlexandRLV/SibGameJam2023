using System;
using GameCore.LevelObjects.FloorTypeDetection;
using UnityEngine;

namespace GameCore.Sounds.ChairMovementSound
{
    [Serializable]
    public class ChairSoundContainer
    {
        [SerializeField] public FloorType floorType;
        [SerializeField] public AudioClip audioClip;
    }
}