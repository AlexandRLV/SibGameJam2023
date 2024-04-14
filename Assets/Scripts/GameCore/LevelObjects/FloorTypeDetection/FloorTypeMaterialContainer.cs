using System;
using UnityEngine;

namespace GameCore.LevelObjects.FloorTypeDetection
{
    [Serializable]
    public class FloorTypeMaterialContainer
    {
        [SerializeField] public FloorType type;
        [SerializeField] public PhysicMaterial material;
    }
}