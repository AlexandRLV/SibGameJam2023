using UnityEngine;

namespace UI.WindowsSystem.WindowTypes.Extra
{
    public class UIRotator : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed;

        private void Awake()
        {
            transform.Rotate(new Vector3(0f, 0f, _rotationSpeed * Time.deltaTime));
        }
    }
}