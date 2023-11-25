using UnityEngine;

namespace GameCore.Enemies.EnemyObject
{
    public class EnemyFOV : MonoBehaviour
    {
        private const int VisionConeResolution = 300;
        private Mesh _visionConeMesh;
        private MeshFilter _meshFilter;

        private int[] _triangles;
        private Vector3[] _vertices;

        public void Init(float viewAngle)
        {
            _meshFilter = GetComponent<MeshFilter>();
            _visionConeMesh = new Mesh();
            transform.localRotation = Quaternion.Euler(0.0f, viewAngle/2, 0.0f);
            
            _triangles = new int[(VisionConeResolution - 1) * 3];
            _vertices = new Vector3[VisionConeResolution + 1];
        }

        public void SetColor(Color newColor)
        {
            GetComponent<MeshRenderer>().material.color = newColor;
        }

        public void DrawFOV(float viewDistance, float viewAngle, LayerMask layerMask)
        {
            _vertices[0] = Vector3.zero;
            viewAngle *= Mathf.Deg2Rad;
            
            float currentangle = -viewAngle;
            float angleIcrement = viewAngle / (VisionConeResolution - 1);

            var forward = transform.forward;
            var right = transform.right;
            var position = transform.position;
            
            for (int i = 0; i < VisionConeResolution; i++)
            {
                float sin = Mathf.Sin(currentangle);
                float cos = Mathf.Cos(currentangle);
                
                var raycastDirection = forward * cos + right * sin;
                var vertForward = Vector3.forward * cos + Vector3.right * sin;
                
                if (Physics.Raycast(position, raycastDirection, out var hit, viewDistance, layerMask))
                {
                    _vertices[i + 1] = vertForward * hit.distance;
                }
                else
                {
                    _vertices[i + 1] = vertForward * viewDistance;
                }

                currentangle += angleIcrement;
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
