using UnityEngine;

namespace GameCore.Enemies.EnemyObject
{
    public class EnemyFOV : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;
        
        private const int VisionConeResolution = 300;
        private Mesh _visionConeMesh;

        private int[] _triangles;
        private Vector3[] _vertices;

        public void Init(float viewAngle)
        {
            _visionConeMesh = new Mesh();
            transform.localRotation = Quaternion.Euler(0.0f, viewAngle/2, 0.0f);
            
            _triangles = new int[(VisionConeResolution - 1) * 3];
            _vertices = new Vector3[VisionConeResolution + 1];
        }

        public void SetColor(Color newColor)
        {
            _meshRenderer.material.color = newColor;
        }

        public void Disable()
        {
            _meshRenderer.enabled = false;
        }

        public void DrawFOV(float viewDistance, float viewAngle, LayerMask layerMask)
        {
            _vertices[0] = Vector3.zero;
            viewAngle *= Mathf.Deg2Rad;
            
            float currentAngle = -viewAngle;
            float angleIncrement = viewAngle / (VisionConeResolution - 1);

            var forward = transform.forward;
            var right = transform.right;
            var position = transform.position;
            
            for (int i = 0; i < VisionConeResolution; i++)
            {
                float sin = Mathf.Sin(currentAngle);
                float cos = Mathf.Cos(currentAngle);
                
                var raycastDirection = forward * cos + right * sin;
                var vertForward = Vector3.forward * cos + Vector3.right * sin;
                
                if (Physics.Raycast(position, raycastDirection, out var hit, viewDistance, layerMask))
                    _vertices[i + 1] = vertForward * hit.distance;
                else
                    _vertices[i + 1] = vertForward * viewDistance;

                currentAngle += angleIncrement;
            }
            
            for (int i = 0, j = 0; i < _triangles.Length; i += 3, j++)
            {
                _triangles[i] = 0;
                _triangles[i + 1] = j + 1;
                _triangles[i + 2] = j + 2;
            }
            
            _visionConeMesh.Clear();
            _visionConeMesh.vertices = _vertices;
            _visionConeMesh.triangles = _triangles;
            
            _meshFilter.mesh = _visionConeMesh;
        }
    }
}
