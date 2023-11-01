using UnityEngine;

namespace UI
{
    public class LoadingScreen : MonoBehaviour
    {
        public bool Active
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }
    }
}