using Common.DI;
using UnityEngine;

namespace GameCore.LevelObjects.FloorTypeDetection
{
    public class FloorTypeDetector : MonoBehaviour
    {
        [SerializeField] private float _yOffset;
        [SerializeField] private float _checkDistance;
        [SerializeField] private LayerMask _floorMask;
        
        [Inject] private FloorTypesConfig _config;

        private void Awake()
        {
            GameContainer.InjectToInstance(this);
        }

        public FloorType GetCurrentType()
        {
            var origin = transform.position + Vector3.up * _yOffset;
            if (!Physics.Raycast(origin, Vector3.down, out var hit, _checkDistance, _floorMask))
                return _config.defaultType;

            if (hit.colliderInstanceID == 0)
                return _config.defaultType;

            var material = hit.collider.sharedMaterial;
            return _config.GetTypeForMaterial(material);
        }
    }
}