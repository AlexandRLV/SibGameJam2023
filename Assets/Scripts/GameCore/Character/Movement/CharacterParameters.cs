using UnityEngine;

namespace GameCore.Character.Movement
{
    [CreateAssetMenu(fileName = "Character Parameters")]
    public class CharacterParameters : ScriptableObject
    {
        [Header("Common")]
        [SerializeField] public float speed;
        [SerializeField] public bool canJump;
        [SerializeField] public float jumpHeight;
        [SerializeField] public float inAirSpeed;
        [SerializeField] public float gravityMultiplier;

        [Header("Inertia")]
        [SerializeField] public float lerpInertiaSpeed;

        [Header("Floating")]
        [SerializeField] public float springForce;
        [SerializeField] public float dampingForce;
    }
}