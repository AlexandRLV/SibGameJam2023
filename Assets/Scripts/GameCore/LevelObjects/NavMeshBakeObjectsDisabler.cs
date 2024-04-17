using UnityEngine;

namespace GameCore.LevelObjects
{
    public class NavMeshBakeObjectsDisabler : MonoBehaviour
    {
        [SerializeField] private GameObject[] _objectsToDisable;

        [ContextMenu("Disable objects")]
        private void DisableObjects()
        {
            foreach (var target in _objectsToDisable)
            {
                target.SetActive(false);
            }
        }

        [ContextMenu("Enable objects")]
        private void EnableObjects()
        {
            foreach (var target in _objectsToDisable)
            {
                target.SetActive(true);
            }
        }
    }
}