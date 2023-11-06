using UnityEngine;

namespace GameCore.Common
{
    internal class Environment : MonoBehaviour
    {
        [Header("Сейчас это ни на что не влияет, для тестов")]
        [SerializeField] private Material _skybox;
        [SerializeField] private Gradient _gradient;
    }
}