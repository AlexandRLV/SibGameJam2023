using UnityEngine;

namespace GameCore.Character.Movement
{
    [CreateAssetMenu(menuName = "Configs/Character Parameters")]
    public class CharacterParameters : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] public float speed;
        [SerializeField] public float standStillTimeToStartAnimation;
        [SerializeField] public float standStillAnimationTime;

        [Header("Lives")]
        [SerializeField] public int lives;

        [Header("Camera")]
        [SerializeField] public float cameraHeight;
        
        [Header("Jump")]
        [SerializeField] public bool canJump;
        [SerializeField] public float jumpHeight;
        [SerializeField] public float inAirSpeed;
        [SerializeField] public float gravityMultiplier;
        [SerializeField] public float jumpTime = 0.3f;

        [Header("Interaction")]
        [SerializeField] public float interactTime;
        [SerializeField] public bool canPush;
        [SerializeField] public float hitTime;

        [Header("Inertia")]
        [SerializeField] public float lerpInertiaSpeed;

        [Header("Floating")]
        [SerializeField] public float springForce;
        [SerializeField] public float dampingForce;
    }
}