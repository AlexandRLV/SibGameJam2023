using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class InteractiveObject : MonoBehaviour, IInteractable
    {
        public virtual void Interact()
        {
            throw new System.NotImplementedException();
        }
    }
}