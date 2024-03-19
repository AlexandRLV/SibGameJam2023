using UnityEngine;

namespace GameCore.Character.Movement
{
    public class CharacterGroundChecker : MonoBehaviour
    {
        [SerializeField] private float _sphereCastRadius;
        [SerializeField] private float _verticalOffset;
        [SerializeField] private float _groundedDistance;
        [SerializeField] private float _inContactDistance;
        
        [Header("Debug")]
        [SerializeField] private bool _showDebug;
        
        private CharacterMovement _movement;
        private CharacterParameters _parameters;
        private Transform _transform;
        
        private RaycastHit _groundHit;
        private RaycastHit _normalCheckHit;
        
        public void Initialize(CharacterMovement movement)
        {
            _movement = movement;
            _parameters = movement.Parameters;
            _transform = transform;
        }
        
        public void CheckGrounded()
        {
            var position = _transform.position;
            var groundCheckOrigin = position + Vector3.up * _verticalOffset;
            var down = Vector3.down;
            
            // Check grounded
            Physics.SphereCast(groundCheckOrigin, _sphereCastRadius, down, out _groundHit, 1000f, _parameters.groundMask);
            
            var firstNormal = _groundHit.normal;
            float firstGroundAngle = _groundHit.colliderInstanceID != 0 ? Vector3.Angle(firstNormal, Vector3.up) : 0f;
            
            float distance = _groundHit.colliderInstanceID != 0 ? groundCheckOrigin.y - _groundHit.point.y : 1000f;
	        
            var secondNormal = GetSurfaceNormal(groundCheckOrigin, down);
            float secondGroundAngle = Vector3.Angle(secondNormal, Vector3.up);

            _movement.MoveValues.groundAngle = firstGroundAngle < secondGroundAngle ? firstGroundAngle : secondGroundAngle;
            _movement.MoveValues.groundNormal = firstGroundAngle < secondGroundAngle ? firstNormal : secondNormal;

            _movement.MoveValues.groundAngle = Mathf.Min(firstGroundAngle, secondGroundAngle);
	        
            _movement.MoveValues.distanceToGround = distance - _verticalOffset;
            
            _movement.MoveValues.isGrounded = _groundHit.colliderInstanceID != 0 && _movement.MoveValues.distanceToGround <= _groundedDistance;
            _movement.MoveValues.inContact = _groundHit.colliderInstanceID != 0 && _movement.MoveValues.distanceToGround <= _inContactDistance;
        }

        private Vector3 GetSurfaceNormal(Vector3 castOrigin, Vector3 direction)
        {
            var origin = castOrigin + direction * _groundHit.distance;
            var dirCenterToHit = _groundHit.point - castOrigin;

            Physics.Raycast(origin, dirCenterToHit, out var hit, dirCenterToHit.magnitude + 1f, _parameters.groundMask);
            var surfaceNormal = hit.colliderInstanceID != 0 ? hit.normal : Vector3.up;

            return surfaceNormal;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_showDebug || !Application.isPlaying) return;

            float distance1 = _groundHit.colliderInstanceID != 0 ? _groundHit.distance : _groundedDistance;
            var groundCheckOrigin = _transform.position + Vector3.up * (_verticalOffset - distance1);
		    
            Gizmos.color = _movement.MoveValues.inContact
                ? Color.green
                : _movement.MoveValues.isGrounded
                    ? Color.blue
                    : Color.red;
		    
            Gizmos.DrawSphere(groundCheckOrigin, _sphereCastRadius);

            if (!_movement.MoveValues.isGrounded) return;
	        
            Debug.DrawRay(
                _groundHit.point,
                _groundHit.normal,
                Color.cyan);
        }
#endif
    }
}