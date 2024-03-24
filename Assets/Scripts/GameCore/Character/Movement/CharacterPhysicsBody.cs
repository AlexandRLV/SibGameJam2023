using Common;
using Common.DI;
using GameCore.Camera;
using UnityEngine;

namespace GameCore.Character.Movement
{
    public class CharacterPhysicsBody : MonoBehaviour
    {
        private const int SolverIterations = 16;

        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CharacterGroundChecker _groundChecker;
        [SerializeField] private CharacterMovement _movement;
        [SerializeField] private bool _debug;
        
        [Inject] private GameCamera _gameCamera;

        private CharacterParameters _parameters;
        
        private Vector3 _movementVector;
        private Collider[] _overlapCapsuleResults;
        
        private void Awake()
        {
	        GameContainer.InjectToInstance(this);
	        _parameters = _movement.Parameters;
            _rigidbody.solverIterations = SolverIterations;
            _overlapCapsuleResults = new Collider[1];
        }
        
        private void FixedUpdate()
        {
            _groundChecker.CheckGrounded();

            if (!_movement.MoveValues.inContact || _movement.MoveValues.groundAngle > _parameters.slideSlopeAngle)
	            _rigidbody.AddForce(Vector3.up * (Physics.gravity.y * _parameters.gravityMultiplier * _rigidbody.mass));

            if (!_movement.IsControlledByPlayer) return;
            
            var horizontalVelocity = _rigidbody.velocity;
            horizontalVelocity.y = 0f;
            
            if (horizontalVelocity.magnitude > 0.1f)
	            _rigidbody.rotation = Quaternion.LookRotation(horizontalVelocity, Vector3.up);
        }
        
        internal void UpdateMovement(Vector2 input)
        {
	        var rotation = _gameCamera.FollowTarget.transform.FlatRotation();
	        _movementVector = new Vector3(input.x, 0f, input.y);
	        _movementVector = rotation * _movementVector;

	        float stepsDelta = 0f;
	        if (_movement.MoveValues.inContact)
	        {
		        stepsDelta = GetStepsDelta(_movementVector);
		        if (_movement.MoveValues.groundAngle < _parameters.maxSlopeAngle &&
		            stepsDelta < _parameters.minStepHeight)
			        stepsDelta = 0f;
		        
		        _rigidbody.position += Vector3.up * stepsDelta;
		        if (stepsDelta > 0f)
			        _rigidbody.position += _movementVector.normalized * _parameters.stepAdditionalDepth;
	        }
	        
	        if (stepsDelta == 0f)
	        {
		        var normal = _movement.MoveValues.groundNormal;
		        if (!Mathf.Approximately(_movement.MoveValues.groundAngle, 0f))
		        {
			        float currentSpeed = _movementVector.magnitude;
			        var projectedMovement = Vector3.ProjectOnPlane(_movementVector, normal);
	        
			        float forwardAngle = Vector3.Angle(_movementVector, Vector3.ProjectOnPlane(normal, Vector3.up));
			        if (forwardAngle > 90f)
			        {
				        float t = Mathf.Clamp01(_movement.MoveValues.groundAngle /
				                                _parameters.maxSlopeAngle);
				        float speedMultiplier = _parameters.slopeSpeedCurve.Evaluate(t);
				        _movementVector = projectedMovement.normalized * (currentSpeed * speedMultiplier);
			        }
			        else
			        {
				        _movementVector = projectedMovement.normalized * currentSpeed;
			        }
		        }
	        }
	        else
	        {
		        _movementVector.y += stepsDelta;
	        }

	        _rigidbody.velocity = LerpVelocityTo(_rigidbody.velocity, _movementVector, Time.deltaTime);
        }

		internal void UpdateMovementInAir(Vector2 input)
		{
			var rotation = _gameCamera.FollowTarget.transform.FlatRotation();
			_movementVector =rotation * new Vector3(input.x, 0f, input.y);

			var currentVelocity = _rigidbody.velocity;
			var horizontalVelocity = new Vector3(currentVelocity.x, 0.0f, currentVelocity.z);
			float verticalVelocity = currentVelocity.y;

			float inAirHorizontalVelocityLimit = Mathf.Max(horizontalVelocity.magnitude, _parameters.inAirControlSpeed);

			float dot = Mathf.Clamp01(Vector3.Dot(horizontalVelocity.normalized, _movementVector.normalized));
			horizontalVelocity = LerpVelocityTo(horizontalVelocity, horizontalVelocity * dot + _movementVector, Time.deltaTime);

			if (horizontalVelocity.magnitude > inAirHorizontalVelocityLimit)
				horizontalVelocity *= inAirHorizontalVelocityLimit / horizontalVelocity.magnitude;

			_rigidbody.velocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
		}

		private float GetStepsDelta(Vector3 movementVector)
		{
			if (movementVector.magnitude < _parameters.stepCheckMovementThreshold)
				return 0f;
			
			// Get horizontal direction of movement
			var direction = movementVector;
			direction.y = 0f;
			direction = direction.normalized;
			
			// Save values for shorter code
			float stepDepth = _parameters.stepDepth;
			float stepHeight = _parameters.stepHeight + 0.05f;
			float sphereRadius = _collider.radius;
			float height = _collider.height;
			var mask = _parameters.groundMask;
			
			// Check if there is a step in front of us
			var position = _rigidbody.position;
			var colliderTop = position + Vector3.up * height;
			var highRayOrigin = colliderTop + direction * stepDepth;
			if (!Physics.SphereCast(highRayOrigin, sphereRadius, Vector3.down, out var hit, height, mask))
				return 0f;

			var surfaceNormal = GetSurfaceNormal(highRayOrigin, Vector3.down, in hit);
			
			// Check if we aren't trying to step on high slope
			float angle = Mathf.Max(Vector3.Angle(hit.normal, Vector3.up), Vector3.Angle(surfaceNormal, Vector3.up));
			if (angle > _parameters.maxSlopeAngle)
				return 0f;
			
			// Check, if we can step on it
			float hitY = hit.point.y;
			float delta = height - (colliderTop.y - hitY);
			if (hitY <= position.y || delta > stepHeight)
				return 0f;
			
			// Check if we have to step on it, and it isn't a slope we can climb
			float lowOffset = _parameters.minStepHeight * 0.5f;
			if (lowOffset < 0.05f) lowOffset = 0.05f;
			var lowRayOrigin = position + Vector3.up * lowOffset;
			var lowRayDirection = hit.point + Vector3.down * 0.05f - lowRayOrigin;
			if (!Physics.Raycast(lowRayOrigin, lowRayDirection, out var lowHit, lowRayDirection.magnitude * 1.5f,
				    mask))
			{
				return 0f;
			}
			
			angle = Vector3.Angle(lowHit.normal, transform.up);
			if (angle < _parameters.maxSlopeAngle)
				return 0f;
			
			// Check if there is enough space to step on
			var finalRayOrigin = highRayOrigin + Vector3.down * (hit.distance - 0.01f);
			var overlapCapsuleTop = finalRayOrigin + Vector3.up * (height - sphereRadius * 2f);
			int overlapResults = Physics.OverlapCapsuleNonAlloc(
				finalRayOrigin,
				overlapCapsuleTop,
				sphereRadius,
				_overlapCapsuleResults,
				mask);

			if (overlapResults > 0)
			{
#if UNITY_EDITOR
				if (_debug)
				{
					Debug.DrawLine(finalRayOrigin + Vector3.down * sphereRadius,
						overlapCapsuleTop + Vector3.up * sphereRadius, Color.red);
					Debug.DrawLine(finalRayOrigin + Vector3.back * sphereRadius,
						finalRayOrigin + Vector3.forward * sphereRadius, Color.red);
					Debug.DrawLine(finalRayOrigin + Vector3.right * sphereRadius,
						finalRayOrigin + Vector3.left * sphereRadius, Color.red);
					Debug.DrawLine(overlapCapsuleTop + Vector3.back * sphereRadius,
						overlapCapsuleTop + Vector3.forward * sphereRadius, Color.red);
					Debug.DrawLine(overlapCapsuleTop + Vector3.right * sphereRadius,
						overlapCapsuleTop + Vector3.left * sphereRadius, Color.red);
				}
#endif
				return 0f;
			}

#if UNITY_EDITOR
			if (_debug)
				Debug.DrawLine(finalRayOrigin, finalRayOrigin + Vector3.up * (height - sphereRadius), Color.green);
#endif
			return delta;
		}

		private Vector3 GetSurfaceNormal(Vector3 castOrigin, Vector3 direction, in RaycastHit startHit)
		{
			var origin = castOrigin + direction * startHit.distance;
			var dirCenterToHit = startHit.point - castOrigin;

			Physics.Raycast(origin, dirCenterToHit, out var hit, dirCenterToHit.magnitude + 1f,
				_parameters.groundMask);
			var surfaceNormal = hit.colliderInstanceID != 0 ? hit.normal : Vector3.up;

			return surfaceNormal;
		}

		private Vector3 LerpVelocityTo(Vector3 origin, Vector3 target, float deltaTime)
        {
	        origin.x = LerpValue(origin.x, target.x, deltaTime);
	        origin.z = LerpValue(origin.z, target.z, deltaTime);

	        origin.y = target.y < origin.y ? LerpValue(origin.y, target.y, deltaTime * _movement.MoveValues.lerpInertiaSpeed) : origin.y;

	        return origin;
        }

        private float LerpValue(float origin, float target, float deltaTime)
        {
	        origin = Mathf.Lerp(origin, target,
		        _movement.MoveValues.lerpInertiaSpeed * deltaTime);

	        if (Mathf.Abs(origin - target) < _parameters.inertiaThreshold)
	        {
		        origin = target;
	        }

	        return origin;
        }
    }
}