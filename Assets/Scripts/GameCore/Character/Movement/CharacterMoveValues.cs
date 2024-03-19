using GameCore.LevelObjects.Abstract;
using UnityEngine;

namespace GameCore.Character.Movement
{
    public class CharacterMoveValues
    {
        public float speedMultiplier;

        public bool isGrounded;
        public bool inContact;
        public float distanceToGround;
        public float groundAngle;
        public Vector3 groundNormal;

        public float lerpInertiaSpeed;

        public bool isKnockdown;
        public bool isHit;
        public bool forceInteract;
        public InteractiveObject currentInteractiveObject;
    }
}