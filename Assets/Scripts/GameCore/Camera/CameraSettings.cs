using UnityEngine;

namespace GameCore.Camera
{
    [CreateAssetMenu(fileName = "Camera Settings")]
    public class CameraSettings : ScriptableObject
    {
        [SerializeField] public bool invertX;
        [SerializeField] public bool invertY;

        [SerializeField] public float minSensitivity;
        [SerializeField] public float maxSensitivity;
        [SerializeField] public float sensitivity;
    }
}