using System.Collections.Generic;
using Common;
using UnityEngine;

namespace GameCore.Player.Network
{
    public class PositionInterpolatorBase<T> : MonoBehaviour where T : IInterpolateSnapshot<T>
    {
        public ref T Current => ref _current;

        [SerializeField] private NetworkParameters _parameters;

        private int _ownerTick;
        private int _interpolatedTick;

        private float _elapsedTime;
        private float _timeToReachTarget;

        private T _previous;
        private T _from;
        private T _to;
        private T _current;

        private bool _currentInitialized;
        private bool _isTeleporting;

        private float _tickTimer;

        private List<T> _snapshots;
        
#region Monobehaviour Methods
		private void Awake()
		{
			_interpolatedTick = 0;
			_ownerTick = _interpolatedTick + _parameters.InterpolationTickDelay;
			_snapshots = new List<T>();
		}

		private void Update()
		{
			// Debug.Log(">>>____________________________");
			// Debug.Log($">>> Updating interpolator, owner tick is: {_ownerTick}, snapshots in buffer: {_positionSnapshots.Count}");

			bool snapshotsChanged = false;
			for (int i = 0; i < _snapshots.Count; i++)
			{
				var snapshot = _snapshots[i];
				// Debug.Log($">>> Processing snapshot {i} with tick {snapshot.OwnerTick}");
				if (_ownerTick < snapshot.Tick)
				{
					// Debug.Log(">>> Skip this snapshot");
					continue;
				}

				if (snapshot.Teleported)
				{
					// Debug.Log(">>> Snapshot is teleported");
					_from = _previous = _to = _current = snapshot;
					snapshotsChanged = true;
					_isTeleporting = true;
				}
				else
				{
					// Debug.Log(">>> Snapshot not teleported, interpolating from it");
					_previous = _to;
					_to = snapshot;
					
					// Debug.Log($">>> Set from interpolated tick to {_interpolatedTick}");
					_current.Tick = _interpolatedTick;
					
					if (_current.Position.IsNaN())
					{
						Debug.LogError(">>> Trying to set _from to current, but current is NaN!!!");
						continue;
					}
					if (_current.Position.IsInfinity())
					{
						Debug.LogError(">>> Trying to set _from to current, but current is infinity!!!");
						continue;
					}
					
					_from = _current;
					snapshotsChanged = true;
					_isTeleporting = false;
				}
				
				_snapshots.RemoveAt(i);
				i--;
			}
			
			if (_isTeleporting) return;

			if (snapshotsChanged)
			{
				_elapsedTime = 0f;
				_timeToReachTarget = (_to.Tick - _from.Tick) * Time.fixedDeltaTime;
				// Debug.Log($">>> Set time to {_timeToReachTarget}");
			}
			
			// This can happen if snapshots is delayed 
			if (_from.Tick > _to.Tick)
			{
				// Debug.LogError($">>> Error in ticks: from {_from.OwnerTick} is greater than to {_to.OwnerTick}!!");
				return;
			}
			
			float t = _elapsedTime / _timeToReachTarget;
			if (float.IsNaN(t) || float.IsInfinity(t))
			{
				// Debug.LogError($">>> Error in interpolation: t is NaN or infinity, {_elapsedTime} / {_timeToReachTarget}");
				t = 0f;
			}

			if (t > _parameters.MaxExtrapolationTimePercent)
				t = _parameters.MaxExtrapolationTimePercent;
			
			// if (_elapsedTime > _timeToReachTarget)
			// 	Debug.Log($">>> Extrapolating with t: {t}");
			// else
			// 	Debug.Log($">>> Interpolating with t: {t}");
			
			InterpolatePosition(t);
			InterpolateRotation(t);
			
			if (!_to.Teleported)
				_current.InterpolateValues(_from, _to, t);
			
			_elapsedTime += Time.deltaTime;
		}

		private void FixedUpdate()
		{
			_ownerTick++;
			_interpolatedTick = _ownerTick - _parameters.InterpolationTickDelay;
		}
#endregion

#region Interface
		internal void SetOwnerTick(int ownerTick)
		{
			// Debug.Log($">>> Received owner tick {ownerTick} when current is {_ownerTick}");
			if (Mathf.Abs(_ownerTick - ownerTick) <= _parameters.TickDivergenceTolerance)
				return;

			// Debug.Log(">>> Owner tick was overrided!");
			_ownerTick = ownerTick;
			_interpolatedTick = _ownerTick - _parameters.InterpolationTickDelay;
		}

		internal void AddSnapshot(T snapshot)
		{
			// Debug.Log($">>> Received new snapshot with tick {snapshot.OwnerTick}, when current is {_ownerTick}");
			if (!_currentInitialized)
			{
				_current = snapshot;
				_currentInitialized = true;
			}
			
			if (_interpolatedTick - snapshot.Tick > _parameters.TickDivergenceTolerance && !snapshot.Teleported)
			{
				// Debug.Log(">>> Skipping this snapshot because of tick");
				return;
			}

			for (int i = 0; i < _snapshots.Count; i++)
			{
				if (snapshot.Tick > _snapshots[i].Tick)
					continue;
				
				// Debug.Log($">>> Inserting new snapshot before {_positionSnapshots[i].OwnerTick}");
				_snapshots.Insert(i, snapshot);
				return;
			}
					
			_snapshots.Add(snapshot);
		}
#endregion

#region Private Methods
		private void InterpolatePosition(float lerpAmount)
		{
			var current = _current;
			if ((_to.Position - _previous.Position).magnitude < _parameters.MovementThreshold)
			{
				if (_to.Position != _from.Position)
					current.Position = Vector3.Lerp(_from.Position, _to.Position, lerpAmount);
			}
			else
			{
				// Debug.Log($">>> Interpolating position from {_from.OwnerTick} to {_to.OwnerTick} with t {lerpAmount}");
				current.Position = Vector3.LerpUnclamped(_from.Position, _to.Position, lerpAmount);
			}

			if (current.Position.IsNaN())
			{
				Debug.LogError(">>> Calculated snapshot position is NaN!!");
				return;
			}

			if (current.Position.IsInfinity())
			{
				Debug.LogError(">>> Calculated snapshot position is infinity!!");
				return;
			}
			
			_current = current;
		}

		private void InterpolateRotation(float lerpAmount)
		{
			var current = _current;
			if (Quaternion.Angle(_to.Rotation, _from.Rotation) < _parameters.RotationAngleThreshold)
			{
				if (_to.Rotation != _from.Rotation)
					current.Rotation = Quaternion.Slerp(_from.Rotation, _to.Rotation, lerpAmount);
			}
			else
			{
				current.Rotation = Quaternion.SlerpUnclamped(_from.Rotation, _to.Rotation, lerpAmount);
			}
			
			if (current.Rotation.IsNaN())
			{
				Debug.LogError(">>> Calculated snapshot rotation is NaN!!");
				return;
			}
			
			if (current.Rotation.IsInfinity())
			{
				Debug.LogError(">>> Calculated snapshot rotation is infinity!!");
				return;
			}

			_current = current;
		}
#endregion
    }
}