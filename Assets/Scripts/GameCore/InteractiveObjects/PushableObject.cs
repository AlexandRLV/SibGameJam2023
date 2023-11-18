using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class PushableObject : BaseTriggerObject
    {
        [SerializeField] private Rigidbody _rigidbody;
        
        protected override void OnPlayerEnter()
        {
            if (Movement.Parameters.canPush)
                _rigidbody.isKinematic = false;
        }

        protected override void OnPlayerExit()
        {
            _rigidbody.isKinematic = true;
        }
    }
}