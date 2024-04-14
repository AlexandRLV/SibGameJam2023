using System.Collections.Generic;
using UnityEngine;

namespace GameCore.LevelObjects.FloorTypeDetection
{
    [CreateAssetMenu(menuName = "Configs/Floor Types")]
    public class FloorTypesConfig : ScriptableObject
    {
        [SerializeField] public FloorType defaultType;
        [SerializeField] public FloorTypeMaterialContainer[] containers;

        private Dictionary<int, FloorType> _floorTypes;
        
        public void Initialize()
        {
            _floorTypes = new Dictionary<int, FloorType>();
            foreach (var container in containers)
            {
                _floorTypes.Add(container.material.GetInstanceID(), container.type);
            }
        }
        
        public FloorType GetTypeForMaterial(PhysicMaterial material)
        {
            if (material == null)
                return defaultType;
            
            int instanceId = material.GetInstanceID();
            return _floorTypes.GetValueOrDefault(instanceId, defaultType);
        }
    }
}