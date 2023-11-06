using Common;
using GameCore.Common;
using UnityEngine;

namespace GameCore.InteractiveObjects
{
    public class PushableObject : BaseTriggerObject
    {
        [SerializeField] private Rigidbody _rigidbody;
        
        protected override void OnPlayerEnter()
        {
            var roundController = GameContainer.InGame.Resolve<RoundController>();
            if (roundController.Stage != RoundStage.FatMouse) return;

            _rigidbody.isKinematic = false;
        }

        protected override void OnPlayerStay()
        {
        }

        protected override void OnPlayerExit()
        {
            _rigidbody.isKinematic = true;
        }
    }
}